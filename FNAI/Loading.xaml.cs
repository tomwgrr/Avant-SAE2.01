using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FNAI
{
    public partial class Loading : Window
    {
        private bool _warningDismissed = false;

        public Loading()
        {
            InitializeComponent();
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
            // Blink "appuyez sur une touche"
            var blink = (Storyboard)Resources["WarningBlink"];
            blink.Begin();

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

        private void DismissWarning()
        {
            _warningDismissed = true;

            var blink = (Storyboard)Resources["WarningBlink"];
            blink.Stop();

            TriggerFlash();

            // Fade out warning
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.6));
            fadeOut.Completed += (_, _) =>
            {
                WarningScreen.Visibility = Visibility.Collapsed;
                ShowLogo();
            };
            WarningScreen.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void ShowLogo()
        {
            // Fade in logo
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(5));
            fadeIn.Completed += (_, _) =>
            {
                // Attend 2s puis fade out
                var wait = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0) };
                wait.Tick += (_, _) => { wait.Stop(); FadeOut(); };
                wait.Start();
            };
            LogoPanel.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void FadeOut()
        {
            TriggerFlash();

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.0));
            fadeOut.BeginTime = TimeSpan.FromSeconds(0.2);
            fadeOut.Completed += (_, _) =>
            {
                var main = new MainWindow();
                main.Show();
                Close();
            };
            BeginAnimation(OpacityProperty, fadeOut);
        }

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