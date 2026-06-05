using System.Media;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FNAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer lobbySong = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            InitialiserMusique();
        }

        private void End(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Option options = new Option();
            options.Show();
        }

        private void Selected(object sender, MouseEventArgs e)
        {
            switch(sender)
            {
                case Button button when button == startButton:startButton.Content = ">Play"; break;
                case Button button when button == loadButton : loadButton.Content = ">Load"; break;
                case Button button when button == optionsButton : optionsButton.Content = ">Options"; break;
                case Button button when button == exitButton : exitButton.Content = ">Exit"; break;
            }

        }

        void InitialiserMusique()
        {
            lobbySong.Open(new Uri(@"Music\LobbyMusic.mp3", UriKind.RelativeOrAbsolute));
            lobbySong.MediaEnded += RelancerLaMusique;
            lobbySong.Play();
        }

        private void RelancerLaMusique(object sender, EventArgs e)
        {
            lobbySong.Position = TimeSpan.Zero;
            lobbySong.Play();
        }

        private void ArreterMusiqueLobby()
        {
            if (lobbySong != null)
            {
                lobbySong.Stop();
            }
        }



        private void Unselect(object sender, MouseEventArgs e)
        {
            switch(sender)
            {
                case Button button when button == startButton : startButton.Content = "Play"; break;
                case Button button when button == loadButton : loadButton.Content = "Load"; break;
                case Button button when button == optionsButton : optionsButton.Content = "Options"; break;
                case Button button when button == exitButton : exitButton.Content = "Exit"; break;
            }
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            ArreterMusiqueLobby();
        }

    }
}