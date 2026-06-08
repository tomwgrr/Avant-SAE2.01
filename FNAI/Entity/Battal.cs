using System;
using System.Windows;
using System.Windows.Threading;

namespace FNAI.Entity
{
    public class Battal : Entity
    {
        #region --- Attributs ---
        private double _patience;                  // Jauge 0 → 100
        private bool _isVisible;                   // Est-il apparu dans la pièce ?
        private PlayGame _page;

        private DispatcherTimer _timerPatience;    // Gère toute la logique de patience, s'exécute chaque seconde
        private DispatcherTimer _timerAttaque;     // Délai avant le jumpscare après apparition

        private int _secondesPasseesCache = 0;     // Compteur pour traquer le temps continu passé sous le bureau

        private const double MAX_PATIENCE = 100.0;
        private const double PATIENCE_PAR_SECONDE = 2.0;       // Ajoute 2 par seconde à la jauge si le joueur n'est pas caché
        private const double PATIENCE_CACHE_PAR_SECONDE = 3.5; // Ajoute 3.5 par seconde à la jauge, APRES 4 secondes consécutives passées sous le bureau
        private const double PATIENCE_BAISSE_PAR_SECONDE = 5.0;// Retire 5 par seconde si Battal est dans la pièce et que le joueur se cache
        #endregion

        #region --- Constructeur ---
        public Battal(int AIscore, PlayGame page) : base(AIscore)
        {
            _patience = 0;
            _isVisible = false;
            _page = page;

            // Timer principal : toutes les secondes, on évalue la jauge
            _timerPatience = new DispatcherTimer();
            _timerPatience.Interval = TimeSpan.FromSeconds(1);
            _timerPatience.Tick += TimerPatience_Tick;
            _timerPatience.Start();
        }
        #endregion

        #region --- IA ---

        /// <summary>
        /// Appelé chaque seconde. Fait monter ou descendre la jauge selon la situation.
        /// </summary>
        private void TimerPatience_Tick(object sender, EventArgs e)
        {
            if (_isVisible)
            {
                // Battal est apparu
                if (_page.IsHidding)
                {
                    // Le joueur se cache, la jauge descend
                    _patience -= PATIENCE_BAISSE_PAR_SECONDE;
                    if (_patience <= 0)
                    {
                        _patience = 0;
                        Disparaitre();
                    }
                }
                else if (_timerAttaque != null && !_timerAttaque.IsEnabled)
                {
                    // Le joueur n'est plus caché, Battal est toujours là, 
                    // et le délai de clémence est dépassé : Jumpscare immédiat.
                    Attack();
                }
            }
            else
            {
                // Battal n'est pas encore apparu
                if (!_page.IsHidding)
                {
                    MonterJauge(PATIENCE_PAR_SECONDE);
                    _secondesPasseesCache = 0; // Reset si le joueur n'est pas caché
                }
                else
                {
                    _secondesPasseesCache++;

                    // Si ça fait 4 secondes ou plus qu'il est caché
                    if (_secondesPasseesCache >= 4)
                    {
                        MonterJauge(PATIENCE_CACHE_PAR_SECONDE);
                    }
                }
            }
        }

        private void MonterJauge(double montant)
        {
            // AIscore influence la vitesse de montée (entre 1 et 20)
            double multiplicateur = 1.0 + (AIscore / 20.0);
            _patience += montant * multiplicateur;

            if (_patience >= MAX_PATIENCE)
            {
                _patience = MAX_PATIENCE;
                Apparaitre();
            }
        }

        public override void Move() { } // Battal n'a pas de positions intermédiaires

        /// <summary>
        /// Appelé quand la jauge est pleine — apparition soudaine dans la pièce.
        /// </summary>
        private void Apparaitre()
        {
            if (_isVisible) return;

            _isVisible = true;
            _page.AfficherBattal(true);
            _page.TriggerFlash();

            // Lance le timer d'attaque : si le joueur ne se cache pas dans le délai → jumpscare
            double delai = Math.Max(1.5, 4.0 - (AIscore / 10.0)); // Entre 1.5s et 4s selon difficulté
            _timerAttaque = new DispatcherTimer();
            _timerAttaque.Interval = TimeSpan.FromSeconds(delai);
            _timerAttaque.Tick += (s, e) =>
            {
                _timerAttaque.Stop();
                if (_isVisible && !_page.IsHidding)
                    Attack();
            };
            _timerAttaque.Start();
        }

        /// <summary>
        /// Le joueur s'est caché à temps — Battal repart.
        /// </summary>
        private void Disparaitre()
        {
            _isVisible = false;
            _timerAttaque?.Stop();
            _page.AfficherBattal(false);
        }

        #endregion

        #region --- Actions ---

        public override void DoAnAction() { }

        public override void Attack()
        {
            jumpscare();
        }

        public override void jumpscare()
        {
            _timerAttaque?.Stop();
            _isVisible = false;
            _patience = 0;
            _page.AfficherBattal(false);
            _page.TriggerFlash();
            _page.EndGame();
        }

        public override void Noise() { }

        #endregion

        #region --- Contrôle depuis PlayGame ---

        /// <summary>
        /// À appeler depuis PlayGame quand isHidding passe à TRUE.
        /// </summary>
        public void OnPlayerHide()
        {
            _secondesPasseesCache = 0;
        }

        /// <summary>
        /// À appeler depuis PlayGame quand isHidding passe à FALSE.
        /// </summary>
        public void OnPlayerReveal()
        {
            _secondesPasseesCache = 0;
        }

        public void Stop()
        {
            _timerPatience?.Stop();
            _timerAttaque?.Stop();
        }

        #endregion
    }
}