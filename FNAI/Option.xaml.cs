using System;
using System.Windows;
using System.Windows.Input;

namespace FNAI
{
    /// <summary>
    /// Logique d'interaction pour la fenêtre Option.xaml.
    /// Gère la configuration de la difficulté de l'IA pour la Custom Night.
    /// </summary>
    public partial class Option : Window
    {
        // Propriétés statiques permettant de transmettre les niveaux de difficulté au moteur de jeu (IUTGame)
        public static int MariusScore { get; private set; } = 1;
        public static int BattalScore { get; private set; } = 0;
        public static int TomScore { get; private set; } = 0;

        // Variables locales pour la gestion interne de la fenêtre
        private int _mariusScore = 10;
        private int _battalScore = 10;
        private int _tomScore = 10;

        public Option()
        {
            InitializeComponent();
            ChargerCurseurPersonnalise();
            RestaurerValeursActuelles();
        }

        /// <summary>
        /// Charge le curseur personnalisé.
        /// </summary>
        private void ChargerCurseurPersonnalise()
        {
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

        /// <summary>
        /// Restaure les scores précédemment sauvegardés pour éviter qu'ils ne se réinitialisent en ouvrant à nouveau le menu.
        /// </summary>
        private void RestaurerValeursActuelles()
        {
            _mariusScore = MariusScore;
            _battalScore = BattalScore;
            _tomScore = TomScore;

            block1Value.Content = _mariusScore.ToString();
            block2Value.Content = _battalScore.ToString();
            block3Value.Content = _tomScore.ToString();
        }

        #region --- Bloc 1 : Marius ---
        private void Block1Left_Click(object sender, RoutedEventArgs e)
        {
            if (_mariusScore > 0) _mariusScore--;
            block1Value.Content = _mariusScore.ToString();
            MariusScore = _mariusScore;
        }

        private void Block1Right_Click(object sender, RoutedEventArgs e)
        {
            if (_mariusScore < 20) _mariusScore++;
            block1Value.Content = _mariusScore.ToString();
            MariusScore = _mariusScore;
        }
        #endregion

        #region --- Bloc 2 : Battal ---
        private void Block2Left_Click(object sender, RoutedEventArgs e)
        {
            if (_battalScore > 0) _battalScore--;
            block2Value.Content = _battalScore.ToString();
            BattalScore = _battalScore;
        }

        private void Block2Right_Click(object sender, RoutedEventArgs e)
        {
            if (_battalScore < 20) _battalScore++;
            block2Value.Content = _battalScore.ToString();
            BattalScore = _battalScore;
        }
        #endregion

        #region --- Bloc 3 : Tom ---
        private void Block3Left_Click(object sender, RoutedEventArgs e)
        {
            if (_tomScore > 0) _tomScore--;
            block3Value.Content = _tomScore.ToString();
            TomScore = _tomScore;
        }

        private void Block3Right_Click(object sender, RoutedEventArgs e)
        {
            if (_tomScore < 20) _tomScore++;
            block3Value.Content = _tomScore.ToString();
            TomScore = _tomScore;
        }
        #endregion

        /// <summary>
        /// Ferme la fenêtre des options et retourne au menu principal.
        /// </summary>
        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}