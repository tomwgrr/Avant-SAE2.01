
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
        private Marius marius; // champ de classe
        private Battal battal;
        private bool isHidding = false;

        public bool IsHidding => isHidding;
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
            // Initialisation de Marius
            marius = new Marius(Option.MariusScore, this); // pas de "Marius" devant
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

   

        //private void CreerNouvellePopup(int pointsDeVie)
        //{
            //PopupInvasion popup = new PopupInvasion(pointsDeVie);
            //popup.DemandeFermeture += (s, e) => { CanvasPopups.Children.Remove(popup); };
           // popup.DemandeDuplication += (s, e) => { CreerNouvellePopup(2); };

            //double xMax = CanvasPopups.ActualWidth > 0 ? CanvasPopups.ActualWidth - 200 : 400;
            //double yMax = CanvasPopups.ActualHeight > 0 ? CanvasPopups.ActualHeight - 120 : 300;

            //double x = random.Next(0, (int)xMax);
            //double y = random.Next(0, (int)yMax);

            //Canvas.SetLeft(popup, x);
            //Canvas.SetTop(popup, y);
            //CanvasPopups.Children.Add(popup);
        //}

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
                // CreerNouvellePopup(pvDeBase);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !isTransitioning && isCameraOn == false) // Vérifie que la barre d'espace est pressée et qu'une transition n'est pas déjà en cours et que la caméra n'est pas activée
            {
                if (e.Key == Key.Space && !isTransitioning)
                {
                    if (!isHidding)
                    {
                        isTransitioning = true;
                        battal.OnPlayerHide();
                        VideoTransition.Visibility = Visibility.Visible;
                        VideoTransition.Position = TimeSpan.Zero;
                        VideoTransition.Play();
                    }
                    else
                    {
                        SceneCachette.Visibility = Visibility.Collapsed;
                        isHidding = false;
                        battal.OnPlayerReveal();
                    }
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
                    }
                    else if (isCameraOn)
                    {
                      
                        Camera_IsClosed();
                    }
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
        /// <summary>
        /// Gère le mouvement de la souris pour faire tourner la caméra en fonction de la position horizontale de la souris dans la fenêtre. Plus la souris est à gauche, plus la caméra regarde vers la gauche, et inversement.   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            
            double mouseX = e.GetPosition(this).X; // Récupère la position X de la souris par rapport à la fenêtre
            double windowWidth = this.ActualWidth; // Récupère la largeur de la fenêtre
            double normalizedX = (mouseX / windowWidth) * 2 - 1; // Normalise la position X entre -1 et 1 en gros pour que -1 soit à gauche, 0 au centre et 1 à droite
            double maxAngle = 25;
            double angleInRadians = (normalizedX * maxAngle) * (Math.PI / 180); //La conversion en radians
            double lookX = Math.Sin(angleInRadians); // Calcule la composante X de la direction de regard en fonction de l'angle
            double lookZ = -Math.Cos(angleInRadians); // Calcule la composante Z de la direction de regard en fonction de l'angle (négatif pour que regarder vers la droite donne une composante Z négative)
            ControlCamera.LookDirection = new Vector3D(lookX, 0, lookZ); // Met à jour la direction de regard de la caméra
        }

        private void Camera_IsClosed()
        {
            isCameraOn = false;
            Camera.Visibility = Visibility.Collapsed;
            CameraMediaVideoPlay.Stop();
            CameraMediaVideoPlay.Visibility = Visibility.Collapsed;
        }

        private void CameraDisplay(object sender, RoutedEventArgs e)
        {
            Button boutonClique = sender as Button;
            if (boutonClique != null && boutonClique.Tag != null)
            {
                string numeroCamera = boutonClique.Tag.ToString();

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
            }
            
        }
    }
}
