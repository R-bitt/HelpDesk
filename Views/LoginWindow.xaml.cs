using System.Data.SqlClient;
using System.Windows;
using HelpDesk.Helpers;
using HelpDesk.Models;
using HelpDesk.Services;

namespace HelpDesk.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) => LoginTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            string login = LoginTextBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                ShowError("Введите логин");
                LoginTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                PasswordInput.Focus();
                return;
            }

            try
            {
                var authService = new AuthService();
                User? user = authService.Authenticate(login, password);

                if (user == null)
                {
                    ShowError("Неверный логин или пароль, либо учётная запись отключена");
                    PasswordInput.Clear();
                    PasswordInput.Focus();
                    return;
                }

                UserSession.Start(user);

                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Не удалось обратиться к базе данных:\n" + ex.Message,
                    "Ошибка SQL Server",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
