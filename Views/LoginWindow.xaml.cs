using System.Windows;
using HelpDesk.Services;

namespace HelpDesk.Views
{
    public partial class LoginWindow : Window
    {
        private readonly bool _demoError;

        public LoginWindow() : this(false)
        {
        }

        public LoginWindow(bool demoError)
        {
            InitializeComponent();
            _demoError = demoError;
            if (demoError)
            {
                Loaded += ShowDemoError;
            }
        }

        private void ShowDemoError(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "admin";
            PasswordBox.Password = "11111";
            LoginButton_Click(LoginButton, new RoutedEventArgs());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (login.Length == 0)
            {
                ShowError("Введите логин");
                LoginTextBox.Focus();
                return;
            }

            if (password.Length == 0)
            {
                ShowError("Введите пароль");
                PasswordBox.Focus();
                return;
            }

            var authService = new AuthService();
            if (!authService.Authenticate(login, password))
            {
                ShowError("Неверный логин или пароль");
                PasswordBox.Clear();
                PasswordBox.Focus();
                return;
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
