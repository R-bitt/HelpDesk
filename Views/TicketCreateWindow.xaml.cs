using System.Windows;
using System.Windows.Controls;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class TicketCreateWindow : Window
    {
        public Ticket? CreatedTicket { get; private set; }

        public TicketCreateWindow() : this(false)
        {
        }

        public TicketCreateWindow(bool demo)
        {
            InitializeComponent();

            if (demo)
            {
                CategoryComboBox.SelectedIndex = 2;
                PriorityComboBox.SelectedIndex = 2;
                DescriptionTextBox.Text = "Нет доступа к интернету в кабинете 305";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (CategoryComboBox.SelectedItem is not ComboBoxItem category)
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

            var priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Средний";

            CreatedTicket = new Ticket
            {
                Category = category.Content.ToString() ?? string.Empty,
                Description = DescriptionTextBox.Text.Trim(),
                Priority = priority,
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
    }
}
