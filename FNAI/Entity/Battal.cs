using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace FNAI.Entity
{
    /// <summary>
    /// Battal — Entité antagoniste principale.
    ///
    /// Parcours des positions :
    ///   0 = Cam1  →  1 = Cam2  →  2 = Cam3  →  3 = Cam5  →  4 = Bureau
    ///
    /// Mécaniques clés :
    ///   • Regarder Battal sur la bonne caméra pendant 4 s le repousse en Cam1.
    ///   • Se cacher le fait patienter (patience ↓) et finit par le faire partir.
    ///   • Camper sous le bureau trop longtemps ralentit la baisse de patience (pénalité anti-camping).
    ///   • Au bureau, ne pas se cacher déclenche un jumpscare après un délai proportionnel à l'IA.
    /// </summary>
    public sealed class Battal : Entity
    {
        // ────────────────────────────────────────────────
        #region Constantes
        // ────────────────────────────────────────────────

        private const double MAX_PATIENCE = 100.0;

        /// <summary>Patience retirée chaque seconde quand le joueur est caché (bureau).</summary>
        private const double PATIENCE_DRAIN_HIDDEN = 10.0;

        /// <summary>Patience rendue chaque seconde quand le joueur n'est PAS caché (bureau).</summary>
        private const double PATIENCE_RESTORE_VISIBLE = 5.0;

        /// <summary>Pénalité anti-camping : patience réajoutée si le joueur campe > seuil.</summary>
        private const double PATIENCE_CAMPING_PENALTY = 1.5;

        /// <summary>Secondes de cache consécutives avant que la pénalité anti-camping s'active.</summary>
        private const int CAMPING_THRESHOLD_SECONDS = 4;

        /// <summary>Secondes à regarder Battal sur la bonne caméra pour le repousser.</summary>
        private const int STARE_REPEL_THRESHOLD_SECONDS = 4;

        /// <summary>Intervalle (secondes) entre chaque tentative de déplacement.</summary>
        private const int MOVEMENT_OPPORTUNITY_SECONDS = 4;

        /// <summary>Délai minimum (secondes) avant jumpscare au bureau.</summary>
        private const double MIN_ATTACK_DELAY = 1.0;

        /// <summary>Délai maximum (secondes) avant jumpscare au bureau.</summary>
        private const double MAX_ATTACK_DELAY = 4.0;

        #endregion

        // ────────────────────────────────────────────────
        #region État interne
        // ────────────────────────────────────────────────

        private double _patience;
        private bool _isVisibleInOffice;
        private bool _isStopped;

        /// <summary>
        /// Position actuelle : 0 = Cam1, 1 = Cam2, 2 = Cam3, 3 = Cam5, 4 = Bureau.
        /// Utiliser <see cref="CurrentPosition"/> pour lire depuis l'extérieur.
        /// </summary>
        private int _currentPosition;

        /// <summary>Secondes consécutives passées caché (réinitialisé dès que le joueur se découvre).</summary>
        private int _consecutiveHiddenSeconds;

        /// <summary>Secondes consécutives où le joueur regarde la bonne caméra.</summary>
        private int _stareSeconds;

        private readonly PlayGame _page;
        private readonly Random _random;
        private readonly DispatcherTimer _logicTimer;
        private readonly DispatcherTimer _movementTimer;
        private DispatcherTimer? _attackTimer;

        #endregion

        // ────────────────────────────────────────────────
        #region Propriétés publiques (lecture seule)
        // ────────────────────────────────────────────────

        public int CurrentPosition => _currentPosition;
        public double CurrentPatience => _patience;
        public bool IsInOffice => _isVisibleInOffice;

        #endregion

        // ────────────────────────────────────────────────
        #region Constructeur
        // ────────────────────────────────────────────────

        public Battal(int aiScore, PlayGame page) : base(aiScore)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _random = new Random();
            _patience = MAX_PATIENCE;

            ResetToStart(updateVisuals: false);
            UpdateCameraVisuals();

            // Boucle logique : 1 s
            _logicTimer = CreateTimer(TimeSpan.FromSeconds(1), LogicTick);
            // Boucle mouvements : 4 s
            _movementTimer = CreateTimer(TimeSpan.FromSeconds(MOVEMENT_OPPORTUNITY_SECONDS), MovementTick);

            Log("Battal initialisé — AIscore = " + AIscore);
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Boucles principales
        // ────────────────────────────────────────────────

        /// <summary>Tick toutes les secondes — gère patience, regard, camping.</summary>
        private void LogicTick(object? sender, EventArgs e)
        {
            if (_isStopped) return;

            TrackHidingStreak();

            if (_currentPosition == 4)
                HandleOfficeTick();
            else if (_currentPosition > 0)
                HandleCameraStareTick();
            else
                _stareSeconds = 0; // En Cam1, pas de mécanique de regard
        }

        /// <summary>Tick toutes les 4 s — tente un déplacement.</summary>
        private void MovementTick(object? sender, EventArgs e)
        {
            if (_isStopped) return;
            Move();
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Suivi de l'état du joueur
        // ────────────────────────────────────────────────

        /// <summary>
        /// Met à jour <see cref="_consecutiveHiddenSeconds"/> chaque seconde.
        /// On incrémente seulement si le joueur est caché ; on remet à 0 dès qu'il ne l'est plus.
        /// </summary>
        private void TrackHidingStreak()
        {
            if (_page.IsHidding)
                _consecutiveHiddenSeconds++;
            else
                _consecutiveHiddenSeconds = 0;
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Logique du bureau (Position 4)
        // ────────────────────────────────────────────────

        private void HandleOfficeTick()
        {
            _stareSeconds = 0; // Regarder les caméras n'a aucun effet quand Battal est au bureau

            if (_page.IsHidding)
            {
                DrainPatience();
                if (_patience <= 0)
                {
                    Log("Patience épuisée — Battal repart.");
                    Disappear();
                }
            }
            else
            {
                // Le joueur est visible : la patience remonte
                _patience = Math.Min(MAX_PATIENCE, _patience + PATIENCE_RESTORE_VISIBLE);
                Log($"Patience remonte → {_patience:F1}");
            }
        }

        /// <summary>
        /// Baisse la patience, avec pénalité réduite si le joueur campe depuis trop longtemps.
        /// </summary>
        private void DrainPatience()
        {
            _patience -= PATIENCE_DRAIN_HIDDEN;

            bool isCamping = _consecutiveHiddenSeconds >= CAMPING_THRESHOLD_SECONDS;
            if (isCamping)
            {
                _patience += PATIENCE_CAMPING_PENALTY;
                Log($"Anti-camping actif (caché depuis {_consecutiveHiddenSeconds}s) — pénalité +{PATIENCE_CAMPING_PENALTY}");
            }

            _patience = Math.Max(0, _patience); // Jamais sous 0
            Log($"Patience → {_patience:F1} (caché depuis {_consecutiveHiddenSeconds}s, camping={isCamping})");
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Logique du regard caméra (Positions 1–3)
        // ────────────────────────────────────────────────

        private void HandleCameraStareTick()
        {
            if (IsPlayerLookingAtMyCamera())
            {
                _stareSeconds++;
                Log($"Joueur regarde Cam{GetCurrentCamId()} depuis {_stareSeconds}s / {STARE_REPEL_THRESHOLD_SECONDS}");

                if (_stareSeconds >= STARE_REPEL_THRESHOLD_SECONDS)
                {
                    Log("Battal repoussé en Cam1 par le regard du joueur.");
                    ResetToStart(updateVisuals: true);
                }
            }
            else
            {
                if (_stareSeconds > 0) Log("Joueur ne regarde plus — compteur regard remis à 0.");
                _stareSeconds = 0;
            }
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Mouvement
        // ────────────────────────────────────────────────

        public override void Move()
        {
            if (_currentPosition >= 4 || _isStopped) return;

            bool roll = _random.Next(1, 21) <= AIscore;
            Log($"Tentative de mouvement — roll={roll}, regardéparjoueur={IsPlayerLookingAtMyCamera()}");

            if (!roll) return;
            if (IsPlayerLookingAtMyCamera())
            {
                Log("Bloqué par le regard du joueur.");
                return;
            }

            _currentPosition++;
            _stareSeconds = 0;
            UpdateCameraVisuals();

            Log($"Battal avance → Position {_currentPosition}");

            if (_currentPosition == 4)
                ArriveInOffice();
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Arrivée au bureau
        // ────────────────────────────────────────────────

        private void ArriveInOffice()
        {
            if (_isVisibleInOffice) return;

            _isVisibleInOffice = true;
            _patience = MAX_PATIENCE;

            _page.AfficherBattal(true);
            _page.TriggerFlash();

            // Délai avant attaque : plus l'IA est haute, plus c'est rapide
            double ratio = Math.Clamp(AIscore / 20.0, 0.0, 1.0);
            double delay = MAX_ATTACK_DELAY - ratio * (MAX_ATTACK_DELAY - MIN_ATTACK_DELAY);

            Log($"Battal au bureau — délai attaque = {delay:F1}s");

            // Toujours nettoyer l'ancien timer avant d'en créer un nouveau
            CancelAttackTimer();

            _attackTimer = CreateTimer(TimeSpan.FromSeconds(delay), (s, e) =>
            {
                _attackTimer?.Stop();
                if (_isVisibleInOffice && !_page.IsHidding)
                {
                    Log("Délai écoulé — attaque !");
                    Attack();
                }
                else
                {
                    Log("Délai écoulé mais joueur caché — pas d'attaque.");
                }
            }, oneShot: true);
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Disparition / Reset
        // ────────────────────────────────────────────────

        private void Disappear()
        {
            Log("Battal disparaît.");
            _isVisibleInOffice = false;
            CancelAttackTimer();
            _page.AfficherBattal(false);
            ResetToStart(updateVisuals: true);
        }

        /// <summary>Remet Battal en Cam1 et remet la patience à max.</summary>
        private void ResetToStart(bool updateVisuals)
        {
            _currentPosition = 0;
            _patience = MAX_PATIENCE;
            _stareSeconds = 0;
            // NE PAS toucher à _consecutiveHiddenSeconds ici —
            // il est géré indépendamment par TrackHidingStreak().

            if (updateVisuals) UpdateCameraVisuals();
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Attaque / Jumpscare
        // ────────────────────────────────────────────────

        public override void DoAnAction() { }
        public override void Noise() { }

        public override void Attack() => Jumpscare();

        public override void jumpscare() => Jumpscare(); // Redirige vers la méthode propre

        private void Jumpscare()
        {
            if (_isStopped) return;
            Log("JUMPSCARE !");

            Stop();

            _isVisibleInOffice = false;
            _page.AfficherBattal(false);
            _page.TriggerFlash();
            _page.EndGame();
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Contrôle externe (depuis PlayGame)
        // ────────────────────────────────────────────────

        /// <summary>
        /// Arrête tous les timers de Battal proprement.
        /// À appeler lors du GameOver ou de la fermeture de la page.
        /// </summary>
        public void Stop()
        {
            if (_isStopped) return;
            _isStopped = true;

            _logicTimer.Stop();
            _movementTimer.Stop();
            CancelAttackTimer();

            Log("Battal stoppé.");
        }

        #endregion

        // ────────────────────────────────────────────────
        #region Helpers privés
        // ────────────────────────────────────────────────

        private bool IsPlayerLookingAtMyCamera()
            => _page.CurrentCameraId == GetCurrentCamId();

        /// <summary>Convertit la position interne en identifiant de caméra affiché dans l'UI.</summary>
        private int GetCurrentCamId() => _currentPosition switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            3 => 5,
            _ => -1  // Bureau ou invalide
        };

        private void UpdateCameraVisuals()
        {
            int camId = GetCurrentCamId();
            _page.BattalCamPosition = camId;
            _page.MettreAJourAffichageBattal();
        }

        private void CancelAttackTimer()
        {
            _attackTimer?.Stop();
            _attackTimer = null;
        }

        /// <summary>Crée et démarre un <see cref="DispatcherTimer"/>.</summary>
        private static DispatcherTimer CreateTimer(TimeSpan interval, EventHandler handler, bool oneShot = false)
        {
            var timer = new DispatcherTimer { Interval = interval };

            if (oneShot)
            {
                // Wrapping pour auto-stop après le premier tick
                timer.Tick += (s, e) =>
                {
                    ((DispatcherTimer)s!).Stop();
                    handler(s, e);
                };
            }
            else
            {
                timer.Tick += handler;
            }

            timer.Start();
            return timer;
        }

        [Conditional("DEBUG")]
        private static void Log(string message)
            => Debug.WriteLine($"[Battal] {DateTime.Now:HH:mm:ss.fff} | {message}");

        #endregion
    }
}