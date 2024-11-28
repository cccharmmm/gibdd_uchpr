using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace gibdd_uchpr.window
{
    public partial class FinesWindow : Window
    {
        public FinesWindow()
        {
            InitializeComponent();
            Loaded += Fine_Loaded;
            LoadDriver();
            LoadCar();
            LoadState();
        }
        private void EditSelectedFine(object sender, RoutedEventArgs e)
        {
            if (FineListBox.SelectedItem is Fines selectedFine)
            {
                var editFineWindow = new EditFineWindow(selectedFine);
                editFineWindow.ShowDialog();
                UpdateFineList();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите штраф для редактирования.");
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FineListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите штраф для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedFine = FineListBox.SelectedItem as Fines;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить штраф с ID: {selectedFine.id}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var fineToDelete = context.Fines.Find(selectedFine.id);

                        if (fineToDelete != null)
                        {
                            context.Fines.Remove(fineToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Штраф успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateFineList();
                        }
                        else
                        {
                            MessageBox.Show("Штраф не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении штрафа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void CreateFine_Click(object sender, RoutedEventArgs e)
        {
            if (DriverComboBox.SelectedItem == null || CarComboBox.SelectedItem == null || StateComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля: водитель, автомобиль и статус штрафа.",
                                "Ошибка заполненности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CostTextBox.Text) || !int.TryParse(CostTextBox.Text, out int cost) || cost < 200)
            {
                MessageBox.Show("Пожалуйста, введите корректную стоимость штрафа (минимум 200 рублей).",
                                "Ошибка стоимости", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new gibddEntities())
            {
                try
                {
                    var newFine = new Fines
                    {
                        driver_id = (DriverComboBox.SelectedItem as Drivers)?.id ?? 0,
                        car_id = (CarComboBox.SelectedItem as Cars)?.id ?? 0,
                        state_id = (StateComboBox.SelectedItem as StateOfFines)?.id ?? 0,
                        cost = cost
                    };

                    context.Fines.Add(newFine);
                    context.SaveChanges();

                    UpdateFineList();

                    MessageBox.Show("Штраф успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении штрафа: {ex.Message}\n{ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string driver = (DriverComboBox.SelectedItem as Drivers)?.last_name;
            string carVIN = (CarComboBox.SelectedItem as Cars)?.VIN;
            string state = (StateComboBox.SelectedItem as StateOfFines)?.name;
            string cost = CostTextBox.Text;

            if (string.IsNullOrWhiteSpace(driver) && string.IsNullOrWhiteSpace(carVIN) &&
                string.IsNullOrWhiteSpace(state) && string.IsNullOrWhiteSpace(cost))
            {
                MessageBox.Show("Заполните хотя бы одно поле для поиска.");
                return;
            }

            using (var context = new gibddEntities())
            {
                var query = context.Fines.Include(v => v.Drivers)
                                               .Include(v => v.Cars)
                                               .Include(v => v.StateOfFines)
                                               .AsQueryable();
                if (!string.IsNullOrWhiteSpace(driver))
                    query = query.Where(v => v.Drivers.last_name.Contains(driver));

                if (!string.IsNullOrWhiteSpace(carVIN))
                    query = query.Where(v => v.Cars.VIN.Contains(carVIN));

                if (!string.IsNullOrWhiteSpace(state))
                    query = query.Where(v => v.StateOfFines.name.Contains(state));

                if (!string.IsNullOrWhiteSpace(cost))
                {
                    if (int.TryParse(cost, out int costValue))
                        query = query.Where(v => v.cost == costValue);
                    else
                        MessageBox.Show("Введите корректную стоимость.");
                }

                var searchResults = query.ToList();
                FineListBox.ItemsSource = searchResults;
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
        private void LoadCar()
        {
            using (var context = new gibddEntities())
            {
                var types = context.Cars.ToList();
                CarComboBox.ItemsSource = types;
            }
        }
        private void LoadState()
        {
            using (var context = new gibddEntities())
            {
                var types = context.StateOfFines.ToList();
                StateComboBox.ItemsSource = types;
            }
        }

        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateFineList();
        }
        private void UpdateFineList()
        {
            using (var context = new gibddEntities())
            {
                var fines = context.Fines
                    .Include(f => f.Drivers)
                    .Include(f => f.Cars) 
                    .Include(f => f.StateOfFines) 
                    .ToList();

                FineListBox.ItemsSource = fines;
            }
        }

        private void Fine_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFineList();
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

        
    }

}
