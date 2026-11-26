using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HelpDesk.Data;
using HelpDesk.Helpers;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class MainWindow : Window
    {
        private List<Ticket> _tickets = new List<Ticket>();

        public MainWindow()
        {
            InitializeComponent();

            LoadStatuses();
            SafeLoadTickets();

            ShowCurrentUser();
            ApplyRolePermissions();
        }

        /// <summary>
        /// Загружает заявки из MS SQL Server через SELECT с JOIN.
        /// </summary>
        private void LoadTickets()
        {
            var tickets = new List<Ticket>();

            const string sql = @"
                SELECT
                    t.Id,
                    t.UserId,
                    t.CategoryId,
                    c.Name AS Category,
                    t.Description,
                    t.PriorityId,
                    p.Name AS Priority,
                    t.StatusId,
                    s.Name AS Status,
                    t.AssignedUserId,
                    t.CreatedAt,
                    t.UpdatedAt
                FROM dbo.Tickets AS t
                INNER JOIN dbo.Categories AS c ON t.CategoryId = c.Id
                INNER JOIN dbo.Priorities AS p ON t.PriorityId = p.Id
                INNER JOIN dbo.Statuses   AS s ON t.StatusId   = s.Id
                ORDER BY t.Id DESC;";

            using (SqlConnection connection = Database.GetConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ticket = new Ticket
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Category = reader["Category"].ToString() ?? string.Empty,
                            Description = reader["Description"].ToString() ?? string.Empty,
                            PriorityId = Convert.ToInt32(reader["PriorityId"]),
                            Priority = reader["Priority"].ToString() ?? string.Empty,
                            StatusId = Convert.ToInt32(reader["StatusId"]),
                            Status = reader["Status"].ToString() ?? string.Empty,
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };

                        if (reader["AssignedUserId"] != DBNull.Value)
                        {
                            ticket.AssignedUserId = Convert.ToInt32(reader["AssignedUserId"]);
                        }

                        if (reader["UpdatedAt"] != DBNull.Value)
                        {
                            ticket.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
                        }

                        tickets.Add(ticket);
                    }
                }
            }

            _tickets = tickets;
            ApplyFilter();
        }

        /// <summary>
        /// Безопасная загрузка: ошибка SQL Server не завершает приложение аварийно.
        /// </summary>
        private void SafeLoadTickets()
        {
            try
            {
                LoadTickets();
            }
            catch (SqlException ex)
            {
                _tickets = new List<Ticket>();
                TicketsGrid.ItemsSource = null;
                CountText.Text = "Показано заявок: 0 из 0";
                StatusText.Text = "Нет подключения к базе данных";

                MessageBox.Show(
                    "Не удалось загрузить заявки из базы данных.\n\n" +
                    "Проверьте SQL Server, имя базы и строку подключения.\n\n" +
                    ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загружает статусы из таблицы Statuses. Пункт «Все» — интерфейсный.
        /// </summary>
        private void LoadStatuses()
        {
            StatusComboBox.Items.Clear();
            StatusComboBox.Items.Add("Все");

            const string sql = "SELECT Name FROM dbo.Statuses ORDER BY Id";

            try
            {
                using (SqlConnection connection = Database.GetConnection())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StatusComboBox.Items.Add(reader["Name"].ToString());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить статусы из базы данных.\n\n" + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            StatusComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Показывает ФИО и роль текущего пользователя в верхней панели.
        /// </summary>
        private void ShowCurrentUser()
        {
            if (UserSession.CurrentUser == null)
            {
                CurrentUserTextBlock.Text = "Пользователь: не определён";
                return;
            }

            CurrentUserTextBlock.Text =
                "Пользователь: " +
                UserSession.CurrentUser.FullName +
                " (" + UserSession.CurrentUser.RoleName + ")";

            Title = "HelpDesk — Главное окно (" + UserSession.CurrentUser.RoleName + ")";
        }

        /// <summary>
        /// Учебное правило доступа:
        /// удаление — только администратор;
        /// редактирование — администратор и специалист.
        /// </summary>
        private void ApplyRolePermissions()
        {
            if (UserSession.CurrentUser == null)
            {
                DeleteTicketButton.IsEnabled = false;
                EditTicketButton.IsEnabled = false;
                return;
            }

            string role = UserSession.CurrentUser.RoleName;
            bool isAdmin = role == "Администратор";
            bool isSpecialist = role == "Специалист";

            DeleteTicketButton.IsEnabled = isAdmin;
            DeleteTicketButton.ToolTip = isAdmin
                ? "Удалить выбранную заявку"
                : "Удаление доступно только администратору";

            EditTicketButton.IsEnabled = isAdmin || isSpecialist;
            EditTicketButton.ToolTip = isAdmin || isSpecialist
                ? "Редактировать выбранную заявку"
                : "Редактирование доступно администратору и специалисту";
        }

        private void ApplyFilter()
        {
            IEnumerable<Ticket> result = _tickets;

            string text = SearchTextBox.Text.Trim().ToLowerInvariant();
            if (text.Length > 0)
            {
                result = result.Where(t =>
                    t.Description.ToLowerInvariant().Contains(text) ||
                    t.Category.ToLowerInvariant().Contains(text) ||
                    t.Status.ToLowerInvariant().Contains(text));
            }

            string status = StatusComboBox.SelectedItem as string ?? "Все";
            if (status != "Все")
            {
                result = result.Where(t => t.Status == status);
            }

            var list = result.ToList();
            TicketsGrid.ItemsSource = list;
            CountText.Text = $"Показано заявок: {list.Count} из {_tickets.Count}";
            StatusText.Text = "Данные загружены из базы данных";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilter();
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            StatusComboBox.SelectedIndex = 0;
            ApplyFilter();
        }

        private void AddTicketButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TicketCreateWindow { Owner = this };
            if (window.ShowDialog() == true && window.CreatedTicket != null)
            {
                var ticket = window.CreatedTicket;
                ticket.Id = _tickets.Count == 0 ? 1 : _tickets.Max(t => t.Id) + 1;
                ticket.UserId = 3; // до ПР №11: временно считаем автором user1
                ticket.CreatedAt = DateTime.Now;

                _tickets.Add(ticket);
                ApplyFilter();
                StatusText.Text = $"Добавлена заявка №{ticket.Id} (без сохранения в БД)";
            }
        }

        private void EditTicketButton_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedTicket();
        }

        private void TicketsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedTicket();
        }

        private void EditSelectedTicket()
        {
            if (TicketsGrid.SelectedItem is not Ticket ticket)
            {
                MessageBox.Show("Выберите заявку для редактирования.", "HelpDesk",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new TicketEditWindow(ticket) { Owner = this };
            if (window.ShowDialog() == true)
            {
                ApplyFilter();
                StatusText.Text = $"Изменена заявка №{ticket.Id} (без сохранения в БД)";
            }
        }

        private void DeleteTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem is not Ticket ticket)
            {
                MessageBox.Show("Выберите заявку для удаления.", "HelpDesk",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить заявку №{ticket.Id}?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _tickets.Remove(ticket);
                ApplyFilter();
                StatusText.Text = $"Удалена заявка №{ticket.Id} (без сохранения в БД)";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UserSession.End();

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
        }
    }
}
