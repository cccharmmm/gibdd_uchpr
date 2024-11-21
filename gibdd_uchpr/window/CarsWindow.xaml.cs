using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace gibdd_uchpr.window
{
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
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CarListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите машину для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCar = CarListBox.SelectedItem as Cars;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить машину с VIN {selectedCar.VIN}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var carToDelete = context.Cars.Find(selectedCar.id);

                        if (carToDelete != null)
                        {
                            context.Cars.Remove(carToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Машина успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateCarList();
                        }
                        else
                        {
                            MessageBox.Show("Машина не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении машины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void GenerateVINButton_Click(object sender, RoutedEventArgs e)
        {
            var driverId = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0;
            var manufacturerId = (ManufacturerComboBox.SelectedItem as ManufacturerType)?.id ?? 0;
            var model = ModelTextBox.Text;
            var year = YearTextBox.Text;
            var weight = int.TryParse(WeightTextBox.Text, out int parsedWeight) ? parsedWeight : 0;
            var colorId = (ColorComboBox.SelectedItem as CarColors)?.id ?? 0;
            var engineTypeId = (EngineComboBox.SelectedItem as EngineTypes)?.id ?? 0;
            var driveTypeId = (TypeOfDriveComboBox.SelectedItem as TypeOfDrive)?.id ?? 0;
            var regionCodeId = (RegionComboBox.SelectedItem as RegionCodes)?.id ?? 0;

            if (manufacturerId == 0 || string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(year) ||
                weight == 0 || engineTypeId == 0 || driveTypeId == 0 || regionCodeId == 0)
            {
                MessageBox.Show("Заполните все обязательные поля для генерации VIN.");
                return;
            }

            string wmi = GenerateWMI(manufacturerId);
            string vds = GenerateVDS(model, year, weight);
            string vis = GenerateVIS(engineTypeId, driveTypeId, regionCodeId);

            string vin = wmi + vds + vis;
            vin = vin.ToUpper().Replace("I", "1").Replace("O", "0").Replace("Q", "0");

            if (vin.Length > 17)
            {
                vin = vin.Substring(0, 17);
            }
            if (vin.Length < 17)
            {
                vin = vin.PadRight(17, '0');
            }

            if (IsVinUnique(vin))
            {
                VINTextBox.Text = vin;
            }
            else
            {
                MessageBox.Show("Генерированный VIN уже существует. Попробуйте снова.");
            }
        }

        private string GenerateWMI(int manufacturerId)
        {
            string manufacturerName = GetManufacturerName(manufacturerId);
            return manufacturerName.Substring(0, Math.Min(3, manufacturerName.Length)).ToUpper();
        }

        private string GetManufacturerName(int manufacturerId)
        {
            switch (manufacturerId)
            {
                case 1: return "BMW";
                case 2: return "Cadillac";
                case 3: return "KIA";
                case 4: return "Volkswagen";
                default: return "Unknown";
            }
        }

        private string GenerateVDS(string model, string year, int weight)
        {
            
            return model.Substring(0, Math.Min(3, model.Length)).ToUpper() +
                   year.Substring(2, 2) +
                   weight.ToString().Substring(0, Math.Min(2, weight.ToString().Length));
        }

        private string GenerateVIS(int engineTypeId, int driveTypeId, int regionCodeId)
        {
            return RandomString(3) +
                   engineTypeId.ToString().Substring(0, 1).ToUpper() +
                   driveTypeId.ToString().Substring(0, 1).ToUpper() +
                   regionCodeId.ToString().Substring(0, 1).ToUpper() +
                   RandomString(3);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rand = new Random();
            return new string(Enumerable.Range(0, length).Select(x => chars[rand.Next(chars.Length)]).ToArray());
        }

        
        private bool IsVinUnique(string vin)
        {
            return true;
        }
        private void CreateCar_Click(object sender, RoutedEventArgs e)
        {
            
            string vin = VINTextBox.Text;
            if (string.IsNullOrWhiteSpace(vin) || !IsVinValid(vin))
            {
                MessageBox.Show("Пожалуйста, введите корректный VIN. VIN должен содержать 17 символов и не включать буквы I, O, Q.",
                                "Ошибка VIN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(YearTextBox.Text) || !int.TryParse(YearTextBox.Text, out int year) || year < 1886 || year > DateTime.Now.Year)
            {
                MessageBox.Show($"Пожалуйста, введите корректный год выпуска автомобиля (1986–{DateTime.Now.Year}).",
                                "Ошибка года выпуска", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DriverComboBox.SelectedItem == null || ManufacturerComboBox.SelectedItem == null || EngineComboBox.SelectedItem == null || TypeOfDriveComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (водитель, производитель, тип двигателя, тип привода).",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                try
                {
                    var newCar = new Cars
                    {
                        driver_id = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0,
                        VIN = vin,
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

        private bool IsVinValid(string vin)
        {
            if (vin.Length != 17) return false;  

            foreach (char c in vin)
            {
                if (!char.IsLetterOrDigit(c) || "IOQ".Contains(c))  
                {
                    return false;
                }
            }
            return true;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string vin = VINTextBox.Text;
            string model = ModelTextBox.Text;
            string manufacturer = (ManufacturerComboBox.SelectedItem as ManufacturerType)?.name;
            string driver = (DriverComboBox.SelectedItem as Drivers)?.last_name;
            string color = (ColorComboBox.SelectedItem as CarColors)?.color_code;
            string engine = (EngineComboBox.SelectedItem as EngineTypes)?.name_ru;
            string driveType = (TypeOfDriveComboBox.SelectedItem as TypeOfDrive)?.name;
            string region = (RegionComboBox.SelectedItem as RegionCodes)?.code.ToString();
            string year = YearTextBox.Text;
            string weight = WeightTextBox.Text;

            // Проверка, что хотя бы одно поле заполнено
            if (string.IsNullOrWhiteSpace(vin) && string.IsNullOrWhiteSpace(model) &&
                string.IsNullOrWhiteSpace(manufacturer) && string.IsNullOrWhiteSpace(driver) &&
                string.IsNullOrWhiteSpace(color) && string.IsNullOrWhiteSpace(engine) &&
                string.IsNullOrWhiteSpace(driveType) && string.IsNullOrWhiteSpace(region) &&
                string.IsNullOrWhiteSpace(year) && string.IsNullOrWhiteSpace(weight))
            {
                MessageBox.Show("Заполните хотя бы одно поле для поиска.");
                return;
            }

            using (var context = new gibddEntities())
            {
                var query = context.Cars.Include(c => c.Drivers)
                                           .Include(c => c.ManufacturerType)
                                           .Include(c => c.CarColors)
                                           .Include(c => c.EngineTypes)
                                           .Include(c => c.TypeOfDrive)
                                           .Include(c => c.RegionCodes)
                                           .AsQueryable();

                if (!string.IsNullOrWhiteSpace(vin))
                {
                    query = query.Where(c => c.VIN.Contains(vin));
                }
                if (!string.IsNullOrWhiteSpace(model))
                {
                    query = query.Where(c => c.model.Contains(model));
                }
                if (!string.IsNullOrWhiteSpace(manufacturer))
                {
                    query = query.Where(c => c.ManufacturerType.name.Contains(manufacturer));
                }
                if (!string.IsNullOrWhiteSpace(driver))
                {
                    query = query.Where(c => c.Drivers.last_name.Contains(driver));
                }
                if (!string.IsNullOrWhiteSpace(color))
                {
                    query = query.Where(c => c.CarColors.color_code.Contains(color));
                }
                if (!string.IsNullOrWhiteSpace(engine))
                {
                    query = query.Where(c => c.EngineTypes.name_ru.Contains(engine));
                }
                if (!string.IsNullOrWhiteSpace(driveType))
                {
                    query = query.Where(c => c.TypeOfDrive.name.Contains(driveType));
                }
                if (!string.IsNullOrWhiteSpace(region) && int.TryParse(region, out int regionCode))
                {
                    query = query.Where(c => c.RegionCodes.code == regionCode);
                }

                if (!string.IsNullOrWhiteSpace(year))
                {
                    if (int.TryParse(year, out int yearValue))
                    {
                        query = query.Where(c => c.year == yearValue.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Введите корректный год.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(weight))
                {
                    if (int.TryParse(weight, out int weightValue))
                    {
                        query = query.Where(c => c.weight == weightValue);
                    }
                    else
                    {
                        MessageBox.Show("Введите корректный вес.");
                    }
                }

                var searchResults = query.ToList();
                CarListBox.ItemsSource = searchResults;
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
