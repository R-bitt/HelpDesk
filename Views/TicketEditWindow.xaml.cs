using System.Windows;
using System.Windows.Controls;
using HelpDesk.Models;

namespace HelpDesk.Views
{
    public partial class TicketEditWindow : Window
    {
        private readonly Ticket _ticket;

        public TicketEditWindow(Ticket ticket) : this(ticket, false)
        {
        }

        public TicketEditWindow(Ticket ticket, bool demo)
        {
            InitializeComponent();
            _ticket = ticket;

            NumberText.Text = $"Заявка № {ticket.Number}";
            DescriptionTextBox.Text = ticket.Description;
            SelectItem(CategoryComboBox, ticket.Category);
            SelectItem(PriorityComboBox, ticket.Priority);
            SelectItem(StatusComboBox, ticket.Status);
        }

        private static void SelectItem(ComboBox comboBox, string value)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem && comboItem.Content?.ToString() == value)
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

            _ticket.Category = category.Content.ToString() ?? string.Empty;
            _ticket.Description = DescriptionTextBox.Text.Trim();
            _ticket.Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Средний";
            _ticket.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Новая";

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
