using System.Windows;
using System.Linq;
using gibdd_uchpr.model;
using System;

namespace gibdd_uchpr.window
{
    public partial class EditLicensesWindow : Window
    {
        private Licenses _licenseToEdit;

        public EditLicensesWindow(Licenses license)
        {
            InitializeComponent();
            _licenseToEdit = license;

            using (var context = new gibddEntities())
            {
                DriverComboBox.ItemsSource = context.Drivers.ToList();
            }

            DriverComboBox.SelectedItem = DriverComboBox.Items
                .OfType<Drivers>()
                .FirstOrDefault(d => d.id == _licenseToEdit.driver_id);
            LicenseDateTextBox.Text = _licenseToEdit.license_date;
            ExpireDateTextBox.Text = _licenseToEdit.expire_date;
            SeriesTextBox.Text = _licenseToEdit.license_series;
            NumberTextBox.Text = _licenseToEdit.license_number;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DriverComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите водителя.",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(LicenseDateTextBox.Text) || !DateTime.TryParse(LicenseDateTextBox.Text, out var licenseDate))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату выдачи.",
                                "Ошибка даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ExpireDateTextBox.Text) || !DateTime.TryParse(ExpireDateTextBox.Text, out var expireDate))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату окончания действия.",
                                "Ошибка даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SeriesTextBox.Text) || string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите серию и номер удостоверения.",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                var licenseToUpdate = context.Licenses.FirstOrDefault(l => l.id == _licenseToEdit.id);

                if (licenseToUpdate != null)
                {
                    licenseToUpdate.driver_id = ((Drivers)DriverComboBox.SelectedItem)?.id ?? 0;
                    licenseToUpdate.license_date = LicenseDateTextBox.Text;
                    licenseToUpdate.expire_date = ExpireDateTextBox.Text;
                    licenseToUpdate.license_series = SeriesTextBox.Text;
                    licenseToUpdate.license_number = NumberTextBox.Text;

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
