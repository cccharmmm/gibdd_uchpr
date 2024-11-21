using System.Windows;
using System.Data.Entity;
using gibdd_uchpr.model;
using System.Linq;
using System;


namespace gibdd_uchpr.window
{
    public partial class EditCarWindow : Window
    {
        private Cars _selectedCar;
        public EditCarWindow(Cars selectedCar)
        {
            InitializeComponent();
            _selectedCar = selectedCar;

            using (var context = new gibddEntities())
            {
                DriverComboBox.ItemsSource = context.Drivers.ToList();
                ManufacturerComboBox.ItemsSource = context.ManufacturerType.ToList();
                ColorComboBox.ItemsSource = context.CarColors.ToList();
                EngineComboBox.ItemsSource = context.EngineTypes.ToList();
                TypeOfDriveComboBox.ItemsSource = context.TypeOfDrive.ToList();
                RegionComboBox.ItemsSource = context.RegionCodes.ToList();
            }

            VINTextBox.Text = _selectedCar.VIN;
            ModelTextBox.Text = _selectedCar.model;
            YearTextBox.Text = _selectedCar.year;
            WeightTextBox.Text = _selectedCar.weight.ToString();
            DriverComboBox.SelectedItem = DriverComboBox.Items
                .OfType<Drivers>()
                .FirstOrDefault(d => d.id == _selectedCar.driver_id);
            ManufacturerComboBox.SelectedItem = ManufacturerComboBox.Items
                .OfType<ManufacturerType>()
                .FirstOrDefault(m => m.id == _selectedCar.manufacturer_id);
            ColorComboBox.SelectedItem = ColorComboBox.Items
                .OfType<CarColors>()
                .FirstOrDefault(c => c.id == _selectedCar.color_id);
            EngineComboBox.SelectedItem = EngineComboBox.Items
                .OfType<EngineTypes>()
                .FirstOrDefault(e => e.id == _selectedCar.engine_type_id);
            TypeOfDriveComboBox.SelectedItem = TypeOfDriveComboBox.Items
                .OfType<TypeOfDrive>()
                .FirstOrDefault(t => t.id == _selectedCar.type_of__drive_id);
            RegionComboBox.SelectedItem = RegionComboBox.Items
                .OfType<RegionCodes>()
                .FirstOrDefault(r => r.id == _selectedCar.code_region_id);

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show($"Пожалуйста, введите корректный год выпуска автомобиля (1886–{DateTime.Now.Year}).",
                                "Ошибка года выпуска", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (DriverComboBox.SelectedItem == null || ManufacturerComboBox.SelectedItem == null ||
                EngineComboBox.SelectedItem == null || TypeOfDriveComboBox.SelectedItem == null ||
                ColorComboBox.SelectedItem == null || RegionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (водитель, производитель, цвет, тип двигателя, тип привода, регион).",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(WeightTextBox.Text) || !int.TryParse(WeightTextBox.Text, out int weight) || weight <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный вес автомобиля (положительное число).",
                                "Ошибка веса", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                var carToUpdate = context.Cars.FirstOrDefault(c => c.id == _selectedCar.id);

                if (carToUpdate != null)
                {
                    carToUpdate.VIN = VINTextBox.Text;
                    carToUpdate.model = ModelTextBox.Text;
                    carToUpdate.year = YearTextBox.Text;
                    carToUpdate.weight = weight;
                    carToUpdate.driver_id = ((Drivers)DriverComboBox.SelectedItem)?.id ?? 0;
                    carToUpdate.manufacturer_id = ((ManufacturerType)ManufacturerComboBox.SelectedItem)?.id ?? 0;
                    carToUpdate.color_id = ((CarColors)ColorComboBox.SelectedItem)?.id ?? 0;
                    carToUpdate.engine_type_id = ((EngineTypes)EngineComboBox.SelectedItem)?.id ?? 0;
                    carToUpdate.type_of__drive_id = ((TypeOfDrive)TypeOfDriveComboBox.SelectedItem)?.id ?? 0;
                    carToUpdate.code_region_id = ((RegionCodes)RegionComboBox.SelectedItem)?.id ?? 0;
                    context.SaveChanges();
                }
            }

            MessageBox.Show("Изменения сохранены!");
            Close();
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
        private bool IsVinValid(string vin)
        {
            if (vin.Length != 17)
                return false;

            string invalidChars = "IOQ";
            foreach (char c in vin)
            {
                if (invalidChars.Contains(c.ToString().ToUpper()))
                    return false;
            }

            return true;
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
      
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
                this.Close();
        }
    }
}
