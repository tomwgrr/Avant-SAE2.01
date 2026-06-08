using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FNAI
{
    /// <summary>
    /// Logique d'interaction pour le menu principal (MainWindow.xaml).
    /// Gère les interactions visuelles de l'interface utilisateur avant le lancement de la partie.
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer lobbySong = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            InitialiserMusique();
            ChargerCurseurPersonnalise();
        }

        /// <summary>
        /// Charge le curseur de souris personnalisé du jeu FNAI.
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

        /// <summary>
        /// Initialise et lance la musique d'ambiance du menu en boucle.
        /// </summary>
        private void InitialiserMusique()
        {
            // Note pour la SAÉ : Les chemins relatifs vers le dossier de build
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

        /// <summary>
        /// Événement déclenché lors du clic sur le bouton Play pour lancer la partie.
        /// </summary>
        private void OpenPlayGame(object sender, RoutedEventArgs e)
        {
            ArreterMusiqueLobby();

            // On ouvre l'écran de jeu (c'est lui qui instanciera le framework IUTGame)
            PlayGame playGame = new PlayGame();
            playGame.Show();
            this.Close();
        }

        /// <summary>
        /// Événement déclenché lors du clic sur le bouton Options.
        /// </summary>
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Option options = new Option();
            options.Show();
        }

        /// <summary>
        /// Quitte proprement l'application.
        /// </summary>
        private void End(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Effet visuel au survol de la souris : Ajoute un indicateur ">" devant le texte du bouton.
        /// </summary>
        private void Selected(object sender, MouseEventArgs e)
        {
            switch (sender)
            {
                case Button button when button == startButton: startButton.Content = ">Play"; break;
                case Button button when button == loadButton: loadButton.Content = ">Load"; break;
                case Button button when button == optionsButton: optionsButton.Content = ">Options"; break;
                case Button button when button == exitButton: exitButton.Content = ">Exit"; break;
            }
        }

        /// <summary>
        /// Effet visuel quand la souris quitte le bouton : Retire l'indicateur ">".
        /// </summary>
        private void Unselect(object sender, MouseEventArgs e)
        {
            switch (sender)
            {
                case Button button when button == startButton: startButton.Content = "Play"; break;
                case Button button when button == loadButton: loadButton.Content = "Load"; break;
                case Button button when button == optionsButton: optionsButton.Content = "Options"; break;
                case Button button when button == exitButton: exitButton.Content = "Exit"; break;
            }
        }
    }
}