using gibdd_uchpr.model;
using System.IO;
using System.Linq;
using System.Windows;

namespace gibdd_uchpr.window
{
    public partial class EditDriverWindow : Window
    {
        private string PhotoFileName { get; set; }
        private Drivers _selectedDriver;
        public EditDriverWindow(Drivers selectedDriver)
        {
            InitializeComponent();
            _selectedDriver = selectedDriver;

            using (var context = new gibddEntities())
            {
                JobComboBox.ItemsSource = context.CompanyJob.ToList();
            }

            NameTextBox.Text = _selectedDriver.name;
            LastNameTextBox.Text = _selectedDriver.last_name;
            MiddleNameTextBox.Text = _selectedDriver.middle_name;
            SeriaTextBox.Text = _selectedDriver.passport_seria;
            NumberTextBox.Text = _selectedDriver.passport_number;
            AddressTextBox.Text = _selectedDriver.address;
            AddressLifeTextBox.Text = _selectedDriver.address_life;
            JobComboBox.SelectedItem = JobComboBox.Items
               .OfType<CompanyJob>()
               .FirstOrDefault(d => d.id == _selectedDriver.job_id);
            PhoneTextBox.Text = _selectedDriver.phone;
            EmailTextBox.Text = _selectedDriver.email;
            DescriptionTextBox.Text = _selectedDriver.description;
            if (!string.IsNullOrEmpty(selectedDriver.photo))
            {
                PhotoFileName = selectedDriver.photo;
                lb1.Content = "загружено";
                lb1.Visibility = Visibility.Visible;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                var driverToUpdate = context.Drivers.FirstOrDefault(c => c.id == _selectedDriver.id);

                if (driverToUpdate != null)
                {
                    driverToUpdate.name = NameTextBox.Text;
                    driverToUpdate.last_name = LastNameTextBox.Text;
                    driverToUpdate.middle_name = MiddleNameTextBox.Text;
                    driverToUpdate.passport_seria = SeriaTextBox.Text;
                    driverToUpdate.passport_number = NumberTextBox.Text;
                    driverToUpdate.address = AddressTextBox.Text;
                    driverToUpdate.address_life = AddressLifeTextBox.Text;
                    driverToUpdate.job_id = ((CompanyJob)JobComboBox.SelectedItem)?.id ?? 0;
                    driverToUpdate.phone = PhoneTextBox.Text;
                    driverToUpdate.email = EmailTextBox.Text;
                    driverToUpdate.photo = PhotoFileName;
                    driverToUpdate.description = DescriptionTextBox.Text;

                    context.SaveChanges();
                }
            }

            MessageBox.Show("Изменения сохранены!");
            Close();
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
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
