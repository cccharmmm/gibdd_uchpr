using gibdd_uchpr.model;
using System.Windows;
using System.Data.Entity;
using System.Linq;
using System;

namespace gibdd_uchpr.window
{
    public partial class ManufacturerTypeWindow : Window
    {
        public ManufacturerTypeWindow()
        {
            InitializeComponent();
            UpdateManufacturerTypeList();
        }
        private void exitButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вышли из системы");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void ManufacturerType_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateManufacturerTypeList();
        }
        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateManufacturerTypeList();
        }
        private void UpdateManufacturerTypeList()
        {
            using (var context = new gibddEntities())
            {
                var manufacturers = context.ManufacturerType.ToList();
                ManufacturersListBox.ItemsSource = manufacturers;
            }
        }
        private void EditSelectedManufacturer(object sender, RoutedEventArgs e)
        {
            if (ManufacturersListBox.SelectedItem is ManufacturerType selectedManufacturer)
            {
                var editManufacturerWindow = new EditManufacturerTypeWindow(selectedManufacturer);
                editManufacturerWindow.ShowDialog();
                UpdateManufacturerTypeList();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите производителя для редактирования.");
            }
        }
        private void CreateManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, укажите наименование.",
                                "Ошибка заполненности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(NameTextBox.Text, @"^[a-zA-Zа-яА-ЯёЁ]+$"))
                { 
                MessageBox.Show("Наименование должно содержать только буквы (латиница или кириллица).",
                                "Ошибка формата имени", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new gibddEntities())
            {
                try
                {
                    var newManufacturer = new ManufacturerType
                    {
                        name = NameTextBox.Text,
                    };

                    context.ManufacturerType.Add(newManufacturer);
                    context.SaveChanges();

                    MessageBox.Show("Производитель успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateManufacturerTypeList();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturersListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите производителя для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedManufacturer = ManufacturersListBox.SelectedItem as ManufacturerType;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить {selectedManufacturer.name}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var manufacturerToDelete = context.ManufacturerType.Find(selectedManufacturer.id);

                        if (manufacturerToDelete != null)
                        {
                            context.ManufacturerType.Remove(manufacturerToDelete);
                            context.SaveChanges();
                            MessageBox.Show("Производитель успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            UpdateManufacturerTypeList();
                        }
                        else
                        {
                            MessageBox.Show("Производитель не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении производителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните поле для поиска.");
                return;
            }

            using (var context = new gibddEntities())
            {
                var manufacturers = context.ManufacturerType
                                            .Where(m => m.name.Contains(name))
                                            .ToList();

                if (manufacturers.Count == 0)
                {
                    MessageBox.Show("Производители не найдены.");
                }
                else
                {
                    ManufacturersListBox.ItemsSource = manufacturers;
                }
            }
        }
    }

}
