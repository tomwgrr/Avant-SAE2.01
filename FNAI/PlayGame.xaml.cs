using FNAI.Entity;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace FNAI
{
    public partial class PlayGame : Window
    {
        private MediaPlayer battalSpeach = new MediaPlayer();
        private MediaPlayer phoneRing = new MediaPlayer();
        private MediaPlayer backgroundMusic = new MediaPlayer();
        private Random random = new Random();
        private DispatcherTimer timerApparition;

        private bool isCameraOn = false;
        private bool isTransitioning = false;
        private Marius marius;
        private Battal battal;

        public int BattalCamPosition { get; set; } = 1;
        private bool isHidding = false;

        public bool IsHidding => isHidding;

     
        public bool IsTabletOpen => isCameraOn;
        public int CurrentCameraId { get; private set; } = 0;

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
            marius = new Marius(Option.MariusScore, this);
            battal = new Battal(Option.BattalScore, this);
        }

        private void InitialiserBattalSpeach()
        {
            battalSpeach.Open(new Uri(@"Music\PhoneCall.mp3", UriKind.RelativeOrAbsolute));
            phoneRing.Open(new Uri(@"Music\phonering.mp3", UriKind.RelativeOrAbsolute));
            phoneRing.Play();
            phoneRing.MediaEnded += (s, e) => { battalSpeach.Play(); };
        }

        public void AfficherBattal(bool visible)
        {
            BattalImage.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
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
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !isTransitioning && isCameraOn == false)
            {
                if (!isHidding)
                {
                    isTransitioning = true;
                    VideoTransition.Visibility = Visibility.Visible;
                    VideoTransition.Position = TimeSpan.Zero;
                    VideoTransition.Play();
                }
                else
                {
                    SceneCachette.Visibility = Visibility.Collapsed;
                    isHidding = false;
                }
            }

            if (e.Key == Key.Tab)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
                backgroundMusic?.Stop();
                battalSpeach?.Stop();
                phoneRing?.Stop();
            }
            if (e.Key == Key.A)
            {
                phoneRing.Stop();
                battalSpeach.Stop();
            }
            if (e.Key == Key.E && !isTransitioning && !isHidding && !isCameraOn)
            {
                isCameraOn = true;
                CameraMediaVideoPlay.Visibility = Visibility.Visible;
                CameraMediaVideoPlay.Position = TimeSpan.Zero;
                CameraMediaVideoPlay.Play();
            }
            if (e.Key == Key.Escape)
            {
                if (Camera1.Visibility == Visibility.Visible ||
                    Camera2.Visibility == Visibility.Visible ||
                    Camera3.Visibility == Visibility.Visible ||
                    Camera4.Visibility == Visibility.Visible ||
                    Camera5.Visibility == Visibility.Visible ||
                    Camera6.Visibility == Visibility.Visible)
                {
                    Camera1.Visibility = Visibility.Collapsed;
                    Camera2.Visibility = Visibility.Collapsed;
                    Camera3.Visibility = Visibility.Collapsed;
                    Camera4.Visibility = Visibility.Collapsed;
                    Camera5.Visibility = Visibility.Collapsed;
                    Camera6.Visibility = Visibility.Collapsed;

                    Camera.Visibility = Visibility.Visible;

                    // Retour sur le plan principal
                    CurrentCameraId = 0;
                    MettreAJourAffichageBattal();
                }
                else if (isCameraOn)
                {
                    Camera_IsClosed();
                }
            }
        }

        private void VideoTransition_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isCameraOn == false)
            {
                isTransitioning = false;
                isHidding = true;
                VideoTransition.Visibility = Visibility.Collapsed;
                SceneCachette.Visibility = Visibility.Visible;
            }
        }

        private void CameraMediaVideoPlay_MediaEnded(object sender, RoutedEventArgs e)
        {
            CameraMediaVideoPlay.Visibility = Visibility.Collapsed;
            Camera.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            backgroundMusic?.Stop();
            battalSpeach?.Stop();
            phoneRing?.Stop();
            battal?.Stop();
            marius?.Stop();
        }

        public void EndGame()
        {
            backgroundMusic?.Stop();
            battalSpeach?.Stop();
            phoneRing?.Stop();

            marius?.Stop();
            battal?.Stop();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            double mouseX = e.GetPosition(this).X;
            double windowWidth = this.ActualWidth;
            double normalizedX = (mouseX / windowWidth) * 2 - 1;
            double maxAngle = 25;
            double angleInRadians = (normalizedX * maxAngle) * (Math.PI / 180);
            double lookX = Math.Sin(angleInRadians);
            double lookZ = -Math.Cos(angleInRadians);
            ControlCamera.LookDirection = new Vector3D(lookX, 0, lookZ);
        }

        private void Camera_IsClosed()
        {
            isCameraOn = false;
            CurrentCameraId = 0; // Sécurité
            Camera.Visibility = Visibility.Collapsed;
            CameraMediaVideoPlay.Stop();
            CameraMediaVideoPlay.Visibility = Visibility.Collapsed;

            MettreAJourAffichageBattal();
        }

        private void CameraDisplay(object sender, RoutedEventArgs e)
        {
            Button boutonClique = sender as Button;
            if (boutonClique != null && boutonClique.Tag != null)
            {
                string numeroCamera = boutonClique.Tag.ToString();

                CurrentCameraId = int.Parse(numeroCamera);

                switch (numeroCamera)
                {
                    case "1":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera1.Visibility = Visibility.Visible;
                        break;
                    case "2":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera2.Visibility = Visibility.Visible;
                        break;
                    case "3":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera3.Visibility = Visibility.Visible;
                        break;
                    case "4":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera4.Visibility = Visibility.Visible;
                        break;
                    case "5":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera5.Visibility = Visibility.Visible;
                        break;
                    case "6":
                        Camera.Visibility = Visibility.Collapsed;
                        Camera6.Visibility = Visibility.Visible;
                        break;
                }

           
                MettreAJourAffichageBattal();
            }
        }

       
        public void MettreAJourAffichageBattal()
        {
            Camera1Battal.Visibility = Visibility.Collapsed;
            Camera2Battal.Visibility = Visibility.Collapsed;
            Camera3Battal.Visibility = Visibility.Collapsed;
            Camera5Battal.Visibility = Visibility.Collapsed;

            if (isCameraOn)
            {
                if (CurrentCameraId == 1 && BattalCamPosition == 1) Camera1Battal.Visibility = Visibility.Visible;
                if (CurrentCameraId == 2 && BattalCamPosition == 2) Camera2Battal.Visibility = Visibility.Visible;
                if (CurrentCameraId == 3 && BattalCamPosition == 3) Camera3Battal.Visibility = Visibility.Visible;
                if (CurrentCameraId == 5 && BattalCamPosition == 5) Camera5Battal.Visibility = Visibility.Visible;
            }
        }
    }
}