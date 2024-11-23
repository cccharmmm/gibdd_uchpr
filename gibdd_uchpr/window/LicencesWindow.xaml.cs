using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using gibdd_uchpr.model;
using System.ComponentModel;

namespace gibdd_uchpr.window
{
    public partial class LicencesWindow : Window
    {
        public LicencesWindow()
        {
            InitializeComponent();
            Loaded += License_Loaded;
            LoadDriver();
        }

        private void EditSelectedLicense(object sender, RoutedEventArgs e)
        {
            if (LicenseListBox.SelectedItem is Licenses selectedLicense)
            {
                var editLicenseWindow = new EditLicensesWindow(selectedLicense);
                editLicenseWindow.ShowDialog();
                UpdateLicenseList();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите ВУ для редактирования.");
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (LicenseListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите водительское удостоверение для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            var selectedLicense = LicenseListBox.SelectedItem as Licenses;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить водительское удостоверение с ID: {selectedLicense.id}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var licenseToDelete = context.Licenses.Find(selectedLicense.id);

                        if (licenseToDelete != null)
                        {
                            context.Licenses.Remove(licenseToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Водительское удостоверение успешно удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateLicenseList();
                        }
                        else
                        {
                            MessageBox.Show("Водительское удостоверение не найдено в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении ВУ: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string licenseDate = LicenseDateTextBox.Text;
            string expireDate = ExpireDateTextBox.Text;
            string series = SeriesTextBox.Text;
            string number = NumberTextBox.Text;
            string driver = (DriverComboBox.SelectedItem as Drivers)?.last_name;

            if (string.IsNullOrWhiteSpace(licenseDate) && string.IsNullOrWhiteSpace(expireDate) &&
                string.IsNullOrWhiteSpace(series) && string.IsNullOrWhiteSpace(number) &&
                string.IsNullOrWhiteSpace(driver))
            {
                MessageBox.Show("Заполните хотя бы одно поле для поиска.");
                return;
            }

            using (var context = new gibddEntities())
            {
                var query = context.Licenses.Include(d => d.Drivers)
                                                  .AsQueryable();

                if (!string.IsNullOrWhiteSpace(licenseDate))
                {
                    query = query.Where(d => d.license_date.Contains(licenseDate));
                }

                if (!string.IsNullOrWhiteSpace(expireDate))
                {
                    query = query.Where(d => d.expire_date.Contains(expireDate));
                }

                if (!string.IsNullOrWhiteSpace(series))
                {
                    query = query.Where(d => d.license_series.Contains(series));
                }

                if (!string.IsNullOrWhiteSpace(number))
                {
                    query = query.Where(d => d.license_number.Contains(number));
                }

                if (!string.IsNullOrWhiteSpace(driver))
                {
                    var driverId = context.Drivers
                                          .Where(d => d.last_name.Contains(driver))
                                          .Select(d => d.id)
                                          .FirstOrDefault();

                    if (driverId > 0)
                    {
                        query = query.Where(d => d.driver_id == driverId);
                    }
                    else
                    {
                        MessageBox.Show("Водитель не найден.");
                        return;
                    }
                }

                var searchResults = query.ToList();
                LicenseListBox.ItemsSource = searchResults;
            }
        }

        private void CreateLicense_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LicenseDateTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpireDateTextBox.Text) ||
                string.IsNullOrWhiteSpace(SeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(NumberTextBox.Text) ||
                DriverComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.",
                                "Ошибка заполненности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParseExact(LicenseDateTextBox.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime licenseDate))
            {
                MessageBox.Show("Пожалуйста, введите дату выдачи в формате dd.MM.yyyy.",
                                "Ошибка формата даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParseExact(ExpireDateTextBox.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime expireDate))
            {
                MessageBox.Show("Пожалуйста, введите дату окончания в формате dd.MM.yyyy.",
                                "Ошибка формата даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (licenseDate > expireDate)
            {
                MessageBox.Show("Дата окончания срока действия должна быть позже даты выдачи.",
                                "Ошибка логики дат", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(SeriesTextBox.Text, out _) || SeriesTextBox.Text.Length != 4)
            {
                MessageBox.Show("Серия должна содержать ровно 4 цифры.",
                                "Ошибка серии", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(NumberTextBox.Text, out _) || NumberTextBox.Text.Length != 6)
            {
                MessageBox.Show("Номер должен содержать ровно 6 цифр.",
                                "Ошибка номера", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                var driverId = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0;

                var existingLicenses = context.Licenses
                    .Where(l => l.driver_id == driverId)
                    .ToList();

                foreach (var license in existingLicenses)
                {
                    if (DateTime.TryParseExact(license.license_date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime existingLicenseDate) &&
                        DateTime.TryParseExact(license.expire_date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime existingExpireDate))
                    {
                        if ((licenseDate >= existingLicenseDate && licenseDate <= existingExpireDate) ||
                            (expireDate >= existingLicenseDate && expireDate <= existingExpireDate))
                        {
                            MessageBox.Show("У водителя уже есть действующее ВУ в этот период. Пожалуйста, выберите другие даты.",
                                            "Ошибка перекрытия дат", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                try
                {
                    var newLicense = new Licenses
                    {
                        license_date = LicenseDateTextBox.Text,
                        expire_date = ExpireDateTextBox.Text,
                        license_series = SeriesTextBox.Text,
                        license_number = NumberTextBox.Text,
                        driver_id = driverId
                    };

                    context.Licenses.Add(newLicense);
                    context.SaveChanges();

                    UpdateLicenseList();

                    MessageBox.Show("ВУ успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении ВУ: {ex.Message}\n{ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadDriver()
        {
            using (var context = new gibddEntities())
            {
                var types = context.Drivers.ToList();
                DriverComboBox.ItemsSource = types;
            }
        }
        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateLicenseList();
        }
        private void UpdateLicenseList()
        {
            using (var context = new gibddEntities())
            {
                var licences = context.Licenses
                    .Include(l => l.Drivers)
                    .OrderBy(l => l.id)
                    .ToList();

                LicenseListBox.ItemsSource = licences;
            }
        }

        private void License_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLicenseList();
        }
        private void driversButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Водители»");
            DriversWindow driversWindow = new DriversWindow();
            driversWindow.Show();
            this.Close();
        }
        private void carsButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Машины»");
            CarsWindow carsWindow = new CarsWindow();
            carsWindow.Show();
            this.Close();
        }
        private void finesButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Штрафы»");
            FinesWindow finesWindow = new FinesWindow();
            finesWindow.Show();
            this.Close();
        }
        private void exitButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вышли из системы");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void ViewLicenseHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLicense = LicenseListBox.SelectedItem as Licenses;
            if (selectedLicense == null)
            {
                MessageBox.Show("Пожалуйста, выберите ВУ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int licenseId = selectedLicense.id;

            var historyWindow = new HistoryOfStatusWindow(licenseId);
            historyWindow.ShowDialog(); 
        }
    }
}
