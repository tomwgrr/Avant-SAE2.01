using System;
using System.Windows;
using System.Windows.Input;

namespace FNAI
{
    public partial class Option : Window
    {
        private int _mariusScore = 1;
        private int _battalScore = 0;
        private int _tomScore = 0;

        public static int MariusScore { get; private set; } = 1;
        public static int BattalScore { get; private set; } = 0;
        public static int TomScore { get; private set; } = 0;

        public Option()
        {
            InitializeComponent();
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

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}