using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FNAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                case Button button when button == startButton : startButton.Content = ">Play"; break;
                case Button button when button == loadButton : loadButton.Content = ">Load"; break;
                case Button button when button == optionsButton : optionsButton.Content = ">Options"; break;
                case Button button when button == exitButton : exitButton.Content = ">Exit"; break;
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
    }
}