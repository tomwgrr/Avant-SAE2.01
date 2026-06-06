using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FNAI
{
    /// <summary>
    /// Logique d'interaction pour PlayGame.xaml
    /// </summary>
    public partial class PlayGame : Window
    {
        private MediaPlayer battalSpeach = new MediaPlayer();
        private MediaPlayer phoneRing = new MediaPlayer();
        private MediaPlayer backgroundMusic = new MediaPlayer();
        private Random random = new Random();
        private DispatcherTimer timerApparition;
        private bool isTransitioning = false;
        public PlayGame()
        {
            InitializeComponent();
            InitialiserBattalSpeach();
            PlayBackGroundMusic();
            timerApparition = new DispatcherTimer();
            timerApparition.Interval = TimeSpan.FromSeconds(1);
            timerApparition.Tick += TimerApparition_Tick;
            timerApparition.Start();
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

            phoneRing.MediaEnded += (s, e) =>
            {
                battalSpeach.Play();
            };

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

        private void TimerApparition_Tick(object sender, EventArgs e)
        {
            if (random.Next(1, 30) == 1)
            {
              
                int pvDeBase = random.Next(1, 3);
                CreerNouvellePopup(pvDeBase);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !isTransitioning)
            {
                if (SceneJeu.Visibility == Visibility.Visible)
                {
                    isTransitioning = true;
                    VideoTransition.Visibility = Visibility.Visible;
                    VideoTransition.Position = TimeSpan.Zero; 
                    VideoTransition.Play();
                }
                else
                {
                    SceneCachette.Visibility = Visibility.Collapsed;
                    SceneJeu.Visibility = Visibility.Visible;
                }
            }
            if (e.Key == Key.Escape)
            {

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                battalSpeach.Stop();
                phoneRing.Stop();
                this.Close();
            }
        }

       
        private void VideoTransition_MediaEnded(object sender, RoutedEventArgs e)
        {
            isTransitioning = false;
            VideoTransition.Visibility = Visibility.Collapsed;
            SceneJeu.Visibility = Visibility.Collapsed;
            SceneCachette.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            backgroundMusic.Stop();
            battalSpeach.Stop();
            phoneRing.Stop();
        }



    }
}