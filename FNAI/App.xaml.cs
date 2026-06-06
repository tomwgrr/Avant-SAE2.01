using System.Windows;

namespace FNAI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loading = new Loading();
            loading.Show();
        }
    }
}