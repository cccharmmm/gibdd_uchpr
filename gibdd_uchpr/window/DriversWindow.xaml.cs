using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.IO;
using System.Windows.Controls;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для DriversWindow.xaml
    /// </summary>
    public partial class DriversWindow : Window
    {
        private string PhotoFileName { get; set; }
        public DriversWindow()
        {
            InitializeComponent();
            Loaded += Driver_Loaded;
            LoadCompanyJob();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DriverListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите водителя для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDriver = DriverListBox.SelectedItem as Drivers;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить водителя: {selectedDriver.last_name} {selectedDriver.name}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var driverToDelete = context.Drivers.Find(selectedDriver.id);

                        if (driverToDelete != null)
                        {
                            context.Drivers.Remove(driverToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Водитель успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateDriverList();
                        }
                        else
                        {
                            MessageBox.Show("Водитель не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateDriverList();
        }
        private void UpdateDriverList()
        {
            using (var context = new gibddEntities())
            {
                var drivers = context.Drivers
                    .Include(d => d.CompanyJob)
                    .OrderBy(d => d.id)
                    .ToList();
                DriverListBox.ItemsSource = drivers;
            }
        }
        private void Driver_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDriverList();
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
            MessageBox.Show("Вы перешли на окно «Водительское удостоверение»");
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

        private void driversButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Водители»");
            DriversWindow driversWindow = new DriversWindow();
            driversWindow.Show();
            this.Close();
        }
        private void LoadCompanyJob()
        {
            using (var context = new gibddEntities())
            {
                var types = context.CompanyJob.ToList();
                JobComboBox.ItemsSource = types;
            }
        }
        private void LoadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Выберите фото"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                string fileName = System.IO.Path.GetFileName(selectedFilePath);
                string targetDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\almaz\\source\\repos\\gibdd_uchpr\\gibdd_uchpr\\images\\");
                string targetFilePath = System.IO.Path.Combine(targetDirectory, fileName);

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(selectedFilePath, targetFilePath, true);

                string fileNameForDatabase = fileName;
                PhotoFileName = fileNameForDatabase;

                lb1.Content = "загружено";
                lb1.Visibility = Visibility.Visible;
            }
        }
        private void CreateDriver_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(SeriaTextBox.Text) ||
                string.IsNullOrWhiteSpace(NumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressLifeTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhotoFileName))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.",
                                "Ошибка заполненности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneTextBox.Text, @"^8\d{10}$"))
            {
                MessageBox.Show("Номер телефона должен начинаться с '8' и содержать ровно 11 цифр.",
                                "Ошибка формата телефона", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(EmailTextBox.Text, @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,6}$"))
            {
                MessageBox.Show("Введите корректный адрес электронной почты.",
                                "Ошибка формата email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(NameTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$") ||
                !System.Text.RegularExpressions.Regex.IsMatch(LastNameTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$") ||
                !System.Text.RegularExpressions.Regex.IsMatch(MiddleNameTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$"))
            {
                MessageBox.Show("Имя, фамилия и отчество должны содержать только буквы (латиница или кириллица).",
                                "Ошибка формата имени", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(AddressTextBox.Text, @"^\d+$") ||
                System.Text.RegularExpressions.Regex.IsMatch(AddressLifeTextBox.Text, @"^\d+$") ||
                System.Text.RegularExpressions.Regex.IsMatch(AddressTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$") ||
                System.Text.RegularExpressions.Regex.IsMatch(AddressLifeTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$"))
            {
                MessageBox.Show("Адрес регистрации и адрес проживания должны содержать и буквы, и цифры.",
                                "Ошибка формата адреса", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhotoFileName))
            {
                MessageBox.Show("Фотография не загружена. Пожалуйста, загрузите фотографию.",
                                "Ошибка фото", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                try
                {
                    var newDriver = new Drivers
                    {
                        name = NameTextBox.Text,
                        last_name = LastNameTextBox.Text,
                        middle_name = MiddleNameTextBox.Text,
                        passport_seria = SeriaTextBox.Text,
                        passport_number = NumberTextBox.Text,
                        address = AddressTextBox.Text,
                        address_life = AddressLifeTextBox.Text,
                        job_id = (JobComboBox.SelectedItem as CompanyJob)?.id ?? 0,
                        phone = PhoneTextBox.Text,
                        email = EmailTextBox.Text,
                        photo = PhotoFileName,
                        description = DescriptionTextBox.Text
                    };

                    context.Drivers.Add(newDriver);
                    context.SaveChanges();

                    MessageBox.Show("Водитель успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateDriverList();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string last_name = LastNameTextBox.Text;
            string middle_name = MiddleNameTextBox.Text;
            string passport_seria = SeriaTextBox.Text;
            string passport_number = NumberTextBox.Text;
            string address = AddressTextBox.Text;
            string address_life = AddressLifeTextBox.Text;
            string phone = PhoneTextBox.Text;
            string email = EmailTextBox.Text;

            string job = (JobComboBox.SelectedItem as CompanyJob)?.company; 

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(last_name) && string.IsNullOrWhiteSpace(middle_name) &&
                string.IsNullOrWhiteSpace(passport_seria) && string.IsNullOrWhiteSpace(passport_number) &&
                string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(address_life) &&
                string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(job))
            {
                MessageBox.Show("Пожалуйста, заполните хотя бы одно поле для поиска.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                var query = context.Drivers.Include(d => d.CompanyJob).AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(d => d.name.Contains(name));
                }
                if (!string.IsNullOrWhiteSpace(last_name))
                {
                    query = query.Where(d => d.last_name.Contains(last_name));
                }
                if (!string.IsNullOrWhiteSpace(middle_name))
                {
                    query = query.Where(d => d.middle_name.Contains(middle_name));
                }
                if (!string.IsNullOrWhiteSpace(passport_seria))
                {
                    query = query.Where(d => d.passport_seria.Contains(passport_seria));
                }
                if (!string.IsNullOrWhiteSpace(passport_number))
                {
                    query = query.Where(d => d.passport_number.Contains(passport_number));
                }
                if (!string.IsNullOrWhiteSpace(address))
                {
                    query = query.Where(d => d.address.Contains(address));
                }
                if (!string.IsNullOrWhiteSpace(address_life))
                {
                    query = query.Where(d => d.address_life.Contains(address_life));
                }
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    query = query.Where(d => d.phone.Contains(phone));
                }
                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(d => d.email.Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(job))
                {
                    query = query.Where(d => d.CompanyJob.company.Contains(job));
                }

                var searchResults = query.OrderBy(d => d.id).ToList();
                DriverListBox.ItemsSource = searchResults;
            }
        }

    }
}
