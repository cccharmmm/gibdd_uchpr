using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;

namespace gibdd_uchpr.window
{
    public partial class EditHistoryWindow : Window
    {
        private HistoryOfStatus _hosToEdit;
        public EditHistoryWindow(HistoryOfStatus hos)
        {
            InitializeComponent();
            _hosToEdit = hos;

            using (var context = new gibddEntities())
            {
                StatusComboBox.ItemsSource = context.StateOfLicenses.ToList();
                LComboBox.ItemsSource = context.Licenses.ToList();
            }
            StatusComboBox.SelectedItem = StatusComboBox.Items
                .OfType<StateOfLicenses>()
                .FirstOrDefault(d => d.id == _hosToEdit.status_id);
            LComboBox.SelectedItem = LComboBox.Items
                .OfType<Licenses>()
                .FirstOrDefault(d => d.id == _hosToEdit.license_id);
            DateOfChangeTextBox.Text = _hosToEdit.date_of_change;
            CommentTextBox.Text = _hosToEdit.comment;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите статус ВУ.",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (LComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите серию ВУ.",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(DateOfChangeTextBox.Text) || !DateTime.TryParse(DateOfChangeTextBox.Text, out var dateOfChange))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату.",
                                "Ошибка даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                var hosToUpdate = context.HistoryOfStatus.FirstOrDefault(l => l.id == _hosToEdit.id);

                if (hosToUpdate != null)
                {
                    hosToUpdate.status_id = ((StateOfLicenses)StatusComboBox.SelectedItem)?.id ?? 0;
                    hosToUpdate.license_id = ((Licenses)LComboBox.SelectedItem)?.id ?? 0;
                    hosToUpdate.date_of_change = DateOfChangeTextBox.Text;
                    hosToUpdate.comment = CommentTextBox.Text;

                    context.SaveChanges();
                    MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении изменений.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
