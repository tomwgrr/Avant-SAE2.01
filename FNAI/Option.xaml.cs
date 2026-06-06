using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FNAI
{
    public partial class Option : Window
    {
        public Option()
        {
            InitializeComponent();
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
        private BitmapImage LoadImage(string name)
        {
            var uri = new Uri($"pack://application:,,,/Assets/{name}", UriKind.Absolute);
            return new BitmapImage(uri);
        }
        private void Block1Left_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Block1Right_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Block2Left_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Block2Right_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Block3Left_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Block3Right_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}