using FNAI.Entity;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FNAI
{
    public partial class PlayGame : Window
    {
        private MediaPlayer battalSpeach = new MediaPlayer();
        private MediaPlayer phoneRing = new MediaPlayer();
        private MediaPlayer backgroundMusic = new MediaPlayer();
        private Random random = new Random();
        private DispatcherTimer timerApparition;

        public PlayGame()
        {
            InitializeComponent();
            InitialiserBattalSpeach();
            PlayBackGroundMusic();
            InitialiserEntity();

            timerApparition = new DispatcherTimer();
            timerApparition.Interval = TimeSpan.FromSeconds(1);
            timerApparition.Tick += TimerApparition_Tick;
            timerApparition.Start();

            try
            {
                var streamInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Giant.cur"));
                if (streamInfo != null)
                    this.Cursor = new Cursor(streamInfo.Stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du curseur : " + ex.Message);
            }
        }

        private void InitialiserEntity()
        {
            Marius marius = new Marius(Option.MariusScore, this);
        }

        private void mutecall(object sender, RoutedEventArgs e)
        {
            phoneRing.Stop();
            battalSpeach.Stop();
        }

        void InitialiserBattalSpeach()
        {
            battalSpeach.Open(new Uri(@"Music\PhoneCall.m4a", UriKind.RelativeOrAbsolute));
            phoneRing.Open(new Uri(@"Music\phonering.mp3", UriKind.RelativeOrAbsolute));
            phoneRing.Play();
            phoneRing.MediaEnded += (s, e) => { battalSpeach.Play(); };
        }

        private void PlayBackGroundMusic()
        {
            backgroundMusic.Open(new Uri(@"Music\AmbianceMusic.mp3", UriKind.RelativeOrAbsolute));
            backgroundMusic.MediaEnded += RelancerLaMusique;
            backgroundMusic.Play();
        }

        private void RelancerLaMusique(object sender, EventArgs e)
        {
            backgroundMusic.Position = TimeSpan.Zero;
            backgroundMusic.Play();
        }

        private void CreerNouvellePopup(int pointsDeVie)
        {
            PopupInvasion popup = new PopupInvasion(pointsDeVie);
            popup.DemandeFermeture += (s, e) => { CanvasPopups.Children.Remove(popup); };
            popup.DemandeDuplication += (s, e) => { CreerNouvellePopup(2); };

            double xMax = CanvasPopups.ActualWidth > 0 ? CanvasPopups.ActualWidth - 200 : 400;
            double yMax = CanvasPopups.ActualHeight > 0 ? CanvasPopups.ActualHeight - 120 : 300;

            double x = random.Next(0, (int)xMax);
            double y = random.Next(0, (int)yMax);

            Canvas.SetLeft(popup, x);
            Canvas.SetTop(popup, y);
            CanvasPopups.Children.Add(popup);
        }

        public void TriggerFlash()
        {
            var flash = new DoubleAnimationUsingKeyFrames();
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.00))));
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.04))));
            flash.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.12))));
            FlashOverlay.BeginAnimation(OpacityProperty, flash);
        }

        private void TimerApparition_Tick(object sender, EventArgs e)
        {
            if (random.Next(1, 300) == 1)
            {
                int pvDeBase = random.Next(1, 3);
                CreerNouvellePopup(pvDeBase);
            }
        }
    }
}