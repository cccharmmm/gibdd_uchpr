using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using gibdd_uchpr.model;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для LicencesWindow.xaml
    /// </summary>
    public partial class LicencesWindow : Window
    {
        public LicencesWindow()
        {
            InitializeComponent();
            Loaded += License_Loaded;
            LoadDriver();
        }
        private void CreateLicense_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new gibddEntities())
            {
                try
                {
                    var newLicense = new Licenses
                    {
                        license_date = LicenseDateTextBox.Text,
                        expire_date = ExpireDateTextBox.Text,
                        license_series = SeriesTextBox.Text,
                        license_number = NumberTextBox.Text,
                        driver_id = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0
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
    }
}
