using System.Linq;
using System.Windows;
using HelpDesk.Helpers;
using HelpDesk.Views;

namespace HelpDesk
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string demo = e.Args.FirstOrDefault(a => a.StartsWith("--demo="))?.Substring(7) ?? "";

            Window window;
            switch (demo)
            {
                case "main":
                    window = new MainWindow();
                    break;
                case "search":
                    window = new MainWindow("search");
                    break;
                case "filter":
                    window = new MainWindow("filter");
                    break;
                case "combined":
                    window = new MainWindow("combined");
                    break;
                case "create":
                    window = new TicketCreateWindow(true);
                    break;
                case "edit":
                    window = new TicketEditWindow(SampleData.CreateTickets().First(), true);
                    break;
                case "error":
                    window = new LoginWindow(true);
                    break;
                default:
                    window = new LoginWindow();
                    break;
            }

            MainWindow = window;
            window.Show();
        }
    }
}
