using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using HelpDesk.Data;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class TicketCreateWindow : Window
    {
        public Ticket? CreatedTicket { get; private set; }

        public TicketCreateWindow()
        {
            InitializeComponent();

            LoadCategories();
            LoadPriorities();
        }

        /// <summary>
        /// Категории загружаются из таблицы Categories.
        /// В Content хранится название, в Tag — Id для будущего INSERT.
        /// </summary>
        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();

            const string sql = "SELECT Id, Name FROM dbo.Categories ORDER BY Name";

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
                            var item = new ComboBoxItem
                            {
                                Content = reader["Name"].ToString(),
                                Tag = Convert.ToInt32(reader["Id"])
                            };

                            CategoryComboBox.Items.Add(item);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDatabaseError("категории", ex);
            }
        }

        /// <summary>
        /// Приоритеты загружаются из таблицы Priorities с учётом SortOrder.
        /// </summary>
        private void LoadPriorities()
        {
            PriorityComboBox.Items.Clear();

            const string sql = @"
                SELECT Id, Name
                FROM dbo.Priorities
                ORDER BY SortOrder";

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
                            var item = new ComboBoxItem
                            {
                                Content = reader["Name"].ToString(),
                                Tag = Convert.ToInt32(reader["Id"])
                            };

                            PriorityComboBox.Items.Add(item);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDatabaseError("приоритеты", ex);
            }

            if (!SelectByName(PriorityComboBox, "Средний") && PriorityComboBox.Items.Count > 0)
            {
                PriorityComboBox.SelectedIndex = 0;
            }
        }

        private static bool SelectByName(ComboBox comboBox, string name)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem && comboItem.Content?.ToString() == name)
                {
                    comboBox.SelectedItem = comboItem;
                    return true;
                }
            }

            return false;
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

            CreatedTicket = new Ticket
            {
                CategoryId = categoryId,
                Category = category.Content?.ToString() ?? string.Empty,
                Description = DescriptionTextBox.Text.Trim(),
                PriorityId = priorityId,
                Priority = priority.Content?.ToString() ?? string.Empty,
                StatusId = 1,
                Status = "Новая"
            };

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

        private void ShowDatabaseError(string what, SqlException ex)
        {
            MessageBox.Show(
                $"Не удалось загрузить {what} из базы данных.\n\n{ex.Message}",
                "Ошибка базы данных",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
