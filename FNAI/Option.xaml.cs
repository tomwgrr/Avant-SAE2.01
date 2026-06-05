using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FNAI
{
    public partial class Option : Window
    {
        public Option()
        {
            InitializeComponent();
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