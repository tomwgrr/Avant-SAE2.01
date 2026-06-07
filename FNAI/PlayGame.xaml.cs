
using FNAI.Entity;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media.Media3D;

namespace FNAI
{
    public partial class PlayGame : Window
    {
        private MediaPlayer battalSpeach = new MediaPlayer();
        private MediaPlayer phoneRing = new MediaPlayer();
        private MediaPlayer backgroundMusic = new MediaPlayer();
        private Random random = new Random();
        private DispatcherTimer timerApparition;
        private bool isTransitioning = false;
        private bool isHidding = false;


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
            Marius marius = new Marius(Option.MariusScore, this);
        }

        private void InitialiserBattalSpeach()
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
            if (e.Key == Key.Space && !isTransitioning) // Vérifie que la barre d'espace est pressée et qu'une transition n'est pas déjà en cours
            {
                if (e.Key == Key.Space && !isTransitioning)
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
            }

            if (e.Key == Key.Escape)
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
        }

        private void VideoTransition_MediaEnded(object sender, RoutedEventArgs e)
        {
            isTransitioning = false;
            isHidding = true;
            VideoTransition.Visibility = Visibility.Collapsed;
            SceneCachette.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            backgroundMusic?.Stop();
            battalSpeach?.Stop();
            phoneRing?.Stop();
          
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
    }
}
