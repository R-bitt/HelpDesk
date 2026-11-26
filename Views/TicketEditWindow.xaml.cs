using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using HelpDesk.Data;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class TicketEditWindow : Window
    {
        private readonly Ticket _ticket;

        public TicketEditWindow(Ticket ticket)
        {
            InitializeComponent();
            _ticket = ticket;

            NumberText.Text = $"Заявка № {ticket.Id}";
            DescriptionTextBox.Text = ticket.Description;

            LoadCategories();
            LoadPriorities();
            LoadStatuses();

            SelectById(CategoryComboBox, ticket.CategoryId);
            SelectById(PriorityComboBox, ticket.PriorityId);
            SelectById(StatusComboBox, ticket.StatusId);
        }

        private void LoadCategories()
        {
            LoadReference(CategoryComboBox, "SELECT Id, Name FROM dbo.Categories ORDER BY Name", "категории");
        }

        private void LoadPriorities()
        {
            LoadReference(PriorityComboBox, "SELECT Id, Name FROM dbo.Priorities ORDER BY SortOrder", "приоритеты");
        }

        private void LoadStatuses()
        {
            LoadReference(StatusComboBox, "SELECT Id, Name FROM dbo.Statuses ORDER BY Id", "статусы");
        }

        /// <summary>
        /// Загружает справочник в ComboBox: Content — название, Tag — Id.
        /// </summary>
        private void LoadReference(ComboBox comboBox, string sql, string what)
        {
            comboBox.Items.Clear();

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
                            comboBox.Items.Add(new ComboBoxItem
                            {
                                Content = reader["Name"].ToString(),
                                Tag = Convert.ToInt32(reader["Id"])
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить {what} из базы данных.\n\n{ex.Message}",
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static void SelectById(ComboBox comboBox, int id)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    comboItem.Tag is int itemId &&
                    itemId == id)
                {
                    comboBox.SelectedItem = comboItem;
                    return;
                }
            }

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (CategoryComboBox.SelectedItem is not ComboBoxItem category ||
                category.Tag is not int categoryId)
            {
                ShowError("Выберите категорию заявки");
                return;
            }

            if (DescriptionTextBox.Text.Trim().Length == 0)
            {
                ShowError("Введите описание заявки");
                DescriptionTextBox.Focus();
                return;
            }

            if (PriorityComboBox.SelectedItem is not ComboBoxItem priority ||
                priority.Tag is not int priorityId)
            {
                ShowError("Выберите приоритет заявки");
                return;
            }

            if (StatusComboBox.SelectedItem is not ComboBoxItem status ||
                status.Tag is not int statusId)
            {
                ShowError("Выберите статус заявки");
                return;
            }

            _ticket.CategoryId = categoryId;
            _ticket.Category = category.Content?.ToString() ?? string.Empty;
            _ticket.Description = DescriptionTextBox.Text.Trim();
            _ticket.PriorityId = priorityId;
            _ticket.Priority = priority.Content?.ToString() ?? string.Empty;
            _ticket.StatusId = statusId;
            _ticket.Status = status.Content?.ToString() ?? string.Empty;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
