using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HelpDesk.Helpers;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class MainWindow : Window
    {
        private readonly List<Ticket> _tickets;
        private readonly string _demoMode;

        public MainWindow() : this(string.Empty)
        {
        }

        public MainWindow(string demoMode)
        {
            InitializeComponent();
            _demoMode = demoMode;
            _tickets = SampleData.CreateTickets();
            CurrentUserText.Text = "Пользователь: admin";

            if (demoMode == "search" || demoMode == "combined")
            {
                SearchTextBox.Text = "принтер";
            }

            if (demoMode == "filter" || demoMode == "combined")
            {
                StatusComboBox.SelectedIndex = 2;
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (TicketsGrid == null)
            {
                return;
            }

            IEnumerable<Ticket> result = _tickets;

            string text = SearchTextBox.Text.Trim().ToLower();
            if (text.Length > 0)
            {
                result = result.Where(t =>
                    t.Description.ToLower().Contains(text) ||
                    t.Category.ToLower().Contains(text) ||
                    t.Status.ToLower().Contains(text));
            }

            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Все";
            if (status != "Все")
            {
                result = result.Where(t => t.Status == status);
            }

            var list = result.ToList();
            TicketsGrid.ItemsSource = list;
            CountText.Text = $"Показано заявок: {list.Count} из {_tickets.Count}";
            StatusText.Text = "Данные загружены из временного хранилища";
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
                window.CreatedTicket.Id = _tickets.Count == 0 ? 1 : _tickets.Max(t => t.Id) + 1;
                window.CreatedTicket.Number = $"HD-2026-{100 + window.CreatedTicket.Id:000000}";
                window.CreatedTicket.Author = "admin";
                window.CreatedTicket.CreatedAt = DateTime.Now;
                _tickets.Add(window.CreatedTicket);
                ApplyFilter();
                StatusText.Text = $"Создана заявка {window.CreatedTicket.Number}";
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
                StatusText.Text = $"Изменена заявка {ticket.Number}";
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

            var result = MessageBox.Show($"Удалить заявку {ticket.Number}?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _tickets.Remove(ticket);
                ApplyFilter();
                StatusText.Text = $"Удалена заявка {ticket.Number}";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
        }
    }
}
