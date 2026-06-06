using System;
using System.Windows;
using System.Windows.Threading;

namespace FNAI.Entity
{
    public class Marius : Entity
    {
        #region --- Attributs ---
        private int position;
        private PlayGame _page;
        private DispatcherTimer _timerPrincipal;
        private DispatcherTimer _timerAttaque;
        public int Position { get => position; set => position = value; }
        #endregion

        #region --- Constructeur ---
        public Marius(int AIscore, PlayGame page) : base(AIscore)
        {
            position = 0;
            _page = page;

            _timerPrincipal = new DispatcherTimer();
            _timerPrincipal.Interval = TimeSpan.FromSeconds(5);
            _timerPrincipal.Tick += (s, e) =>
            {
                Move();
            };
            _timerPrincipal.Start();
        }
        #endregion

        #region --- IA ---
        public override void Move()
        {
            Random rng = new Random();
            int roll = rng.Next(1, 21);

            bool aBouge = false;

            if (roll <= AIscore)
            {
                if (position < 3)
                {
                    position++;
                    aBouge = true;

                    if (position == 3)
                        LancerTimerAttaque();
                }
            }

            _timerPrincipal.Interval = position switch
            {
                0 => TimeSpan.FromSeconds(5),
                1 => TimeSpan.FromSeconds(4),
                2 => TimeSpan.FromSeconds(2),
                3 => TimeSpan.FromSeconds(1),
                _ => TimeSpan.FromSeconds(5)
            };

            if (aBouge)
                AfficherMarius(_page);
        }

        private void LancerTimerAttaque()
        {
            _timerAttaque?.Stop();

            Random rng = new Random();
            double maxAttente = 20.0 / AIscore;
            double attente = rng.NextDouble() * maxAttente + 1;

            _timerAttaque = new DispatcherTimer();
            _timerAttaque.Interval = TimeSpan.FromSeconds(attente);
            _timerAttaque.Tick += (s, e) =>
            {
                _timerAttaque.Stop();
                if (position == 3)
                    Attack();
            };
            _timerAttaque.Start();
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
            position = 0;
            _timerAttaque?.Stop();
            AfficherMarius(_page);
            _timerPrincipal.Interval = TimeSpan.FromSeconds(5);
            MessageBox.Show("JUMPSCARE !");
        }

        public void Flash()
        {
            if (position == 3)
            {
                _timerAttaque?.Stop();

                position = 4;
                AfficherMarius(_page);

                DispatcherTimer timerFlash = new DispatcherTimer();
                timerFlash.Interval = TimeSpan.FromSeconds(1);
                timerFlash.Tick += (s, e) =>
                {
                    timerFlash.Stop();
                    jumpscare();
                };
                timerFlash.Start();
            }
        }

        public override void Noise() { }
        #endregion

        #region --- Affichage ---
        public void AfficherMarius(PlayGame page)
        {
            _page = page;

            page.FarAwayStandingMarius.Visibility = Visibility.Collapsed;
            page.StandingMarius.Visibility = Visibility.Collapsed;
            page.MariusOnDesk.Visibility = Visibility.Collapsed;
            page.MariusFlashOnDesk.Visibility = Visibility.Collapsed;

            page.TriggerFlash();

            switch (position)
            {
                case 0: break;
                case 1: page.FarAwayStandingMarius.Visibility = Visibility.Visible; break;
                case 2: page.StandingMarius.Visibility = Visibility.Visible; break;
                case 3: page.MariusOnDesk.Visibility = Visibility.Visible; break;
                case 4: page.MariusFlashOnDesk.Visibility = Visibility.Visible; break;
            }
        }
        #endregion
    }
}