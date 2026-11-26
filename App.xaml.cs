using System.Windows;
using HelpDesk.Views;

namespace HelpDesk
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new LoginWindow();
            MainWindow = window;
            window.Show();
        }
    }
}
