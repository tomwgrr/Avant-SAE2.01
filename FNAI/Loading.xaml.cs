using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FNAI
{
    /// <summary>
    /// Logique d'interaction pour l'écran de chargement (IHM pure).
    /// Cette classe gère l'introduction visuelle avant le lancement du moteur de jeu.
    /// </summary>
    public partial class Loading : Window
    {
        private bool _warningDismissed = false;

        public Loading()
        {
            InitializeComponent();
            ChargerCurseurPersonnalise();
        }

        /// <summary>
        /// Charge le curseur personnalisé du jeu.
        /// </summary>
        private void ChargerCurseurPersonnalise()
        {
            try
            {
                var streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Giant.cur"));
                if (streamInfo != null)
                {
                    this.Cursor = new Cursor(streamInfo.Stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du curseur : " + ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Lance le clignotement de l'avertissement défini dans le XAML
            var blink = (Storyboard)Resources["WarningBlink"];
            blink.Begin();

            // Écoute des entrées utilisateurs pour passer l'écran
            KeyDown += OnAnyKey;
            MouseDown += OnAnyClick;
        }

        private void OnAnyKey(object sender, KeyEventArgs e)
        {
            if (_warningDismissed) return;
            DismissWarning();
        }

        private void OnAnyClick(object sender, MouseButtonEventArgs e)
        {
            if (_warningDismissed) return;
            DismissWarning();
        }

        /// <summary>
        /// Supprime l'écran d'avertissement avec un effet visuel et passe au logo.
        /// </summary>
        private void DismissWarning()
        {
            _warningDismissed = true;

            var blink = (Storyboard)Resources["WarningBlink"];
            blink.Stop();

            TriggerFlash();

            // Fondu enchaîné (Fade out) de l'avertissement
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.6));
            fadeOut.Completed += (_, _) =>
            {
                WarningScreen.Visibility = Visibility.Collapsed;
                ShowLogo();
            };
            WarningScreen.BeginAnimation(OpacityProperty, fadeOut);
        }

        /// <summary>
        /// Affiche le logo du jeu FNAI avec un fondu.
        /// </summary>
        private void ShowLogo()
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            fadeIn.Completed += (_, _) =>
            {
                // Pause de 0.5s avant de lancer le fondu de fermeture
                var wait = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                wait.Tick += (_, _) => { wait.Stop(); FadeOut(); };
                wait.Start();
            };
            LogoPanel.BeginAnimation(OpacityProperty, fadeIn);
        }

        /// <summary>
        /// Quitte l'écran de chargement et ouvre le menu principal.
        /// </summary>
        private void FadeOut()
        {
            TriggerFlash();

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.BeginTime = TimeSpan.FromSeconds(0.2);
            fadeOut.Completed += (_, _) =>
            {
                // Transition vers le menu principal
                MainWindow main = new MainWindow();
                main.Show();
                Close();
            };
            BeginAnimation(OpacityProperty, fadeOut);
        }

        /// <summary>
        /// Effet d'éclair / flash de lumière typique de l'ambiance horreur.
        /// </summary>
        private void TriggerFlash()
        {
            var flash = new DoubleAnimationUsingKeyFrames();
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.00))));
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.04))));
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.12))));
            FlashOverlay.BeginAnimation(OpacityProperty, flash);
        }
    }
}