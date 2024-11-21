using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для CarsWindow.xaml
    /// </summary>
    public partial class CarsWindow : Window
    {
        public CarsWindow()
        {
            InitializeComponent();
            Loaded += Car_Loaded;
            LoadRegion();
            LoadTypeOfDrive();
            LoadEngine();
            LoadColor();
            LoadManufacturer();
            LoadDriver();
        }
        private void CreateCar_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new gibddEntities())
            {
                try
                {
                    var newCar = new Cars
                    {
                        driver_id = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0,
                        VIN = VINTextBox.Text,
                        manufacturer_id = (ManufacturerComboBox.SelectedItem as ManufacturerType)?.id ?? 0,
                        model = ModelTextBox.Text,
                        year = YearTextBox.Text,
                        weight = int.TryParse(WeightTextBox.Text, out int weight) ? weight : 0,
                        color_id = (ColorComboBox.SelectedItem as CarColors)?.id ?? 0,
                        engine_type_id = (EngineComboBox.SelectedItem as EngineTypes)?.id ?? 0,
                        type_of__drive_id = (TypeOfDriveComboBox.SelectedItem as TypeOfDrive)?.id ?? 0,
                        code_region_id = (RegionComboBox.SelectedItem as RegionCodes)?.id ?? 0
                    };

                    context.Cars.Add(newCar);
                    context.SaveChanges();

                    UpdateCarList();

                    MessageBox.Show("Машина успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении машины: {ex.Message}\n{ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void UpdateCarList()
        {
            using (var context = new gibddEntities())
            {
                var cars = context.Cars
                    .Include(c => c.Drivers)           
                    .Include(c => c.ManufacturerType)     
                    .Include(c => c.CarColors)            
                    .Include(c => c.EngineTypes)       
                    .Include(c => c.TypeOfDrive)       
                    .Include(c => c.RegionCodes)           
                    .ToList();

                CarListBox.ItemsSource = cars;
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
        private void LoadManufacturer()
        {
            using (var context = new gibddEntities())
            {
                var types = context.ManufacturerType.ToList();
                ManufacturerComboBox.ItemsSource = types;
            }
        }
        private void LoadColor()
        {
            using (var context = new gibddEntities())
            {
                var types = context.CarColors.ToList();
                ColorComboBox.ItemsSource = types;
            }
        }
        private void LoadEngine()
        {
            using (var context = new gibddEntities())
            {
                var types = context.EngineTypes.ToList();
                EngineComboBox.ItemsSource = types;
            }
        }
        private void LoadTypeOfDrive()
        {
            using (var context = new gibddEntities())
            {
                var types = context.TypeOfDrive.ToList();
                TypeOfDriveComboBox.ItemsSource = types;
            }
        }
        private void LoadRegion()
        {
            using (var context = new gibddEntities())
            {
                var types = context.RegionCodes.ToList();
                RegionComboBox.ItemsSource = types;
            }
        }
        private void Car_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCarList();
        }
        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateCarList();
        }
        private void driversButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Водители»");
            DriversWindow driversWindow = new DriversWindow();
            driversWindow.Show();
            this.Close();
        }
        private void finesButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Штрафы»");
            FinesWindow finesWindow = new FinesWindow();
            finesWindow.Show();
            this.Close();
        }
        private void licencesButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Водительское удостоверениев»");
            LicencesWindow licencesWindow = new LicencesWindow();
            licencesWindow.Show();
            this.Close();
        }
        private void exitButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вышли из системы");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void carsButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Машины»");
            CarsWindow carsWindow = new CarsWindow();
            carsWindow.Show();
            this.Close();
        }
    }
}
