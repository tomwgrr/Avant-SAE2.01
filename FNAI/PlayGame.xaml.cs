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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        public PlayGame()
        {
            InitializeComponent();
            InitialiserBattalSpeach();
            PlayBackGroundMusic();
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
    }
}