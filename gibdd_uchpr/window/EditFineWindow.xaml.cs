using System.Windows;
using gibdd_uchpr.model;
using System.Linq;

namespace gibdd_uchpr.window
{
    public partial class EditFineWindow : Window
    {
        private Fines _selectedFine;

        public EditFineWindow(Fines selectedFine)
        {
            InitializeComponent();
            _selectedFine = selectedFine;

            using (var context = new gibddEntities())
            {
                DriverComboBox.ItemsSource = context.Drivers.ToList();
                CarComboBox.ItemsSource = context.Cars.ToList();
                StateComboBox.ItemsSource = context.StateOfFines.ToList();
            }

            DriverComboBox.SelectedItem = DriverComboBox.Items
                .OfType<Drivers>()
                .FirstOrDefault(d => d.id == _selectedFine.driver_id);
            CarComboBox.SelectedItem = CarComboBox.Items
                .OfType<Cars>()
                .FirstOrDefault(c => c.id == _selectedFine.car_id);
            StateComboBox.SelectedItem = StateComboBox.Items
                .OfType<StateOfFines>()
                .FirstOrDefault(s => s.id == _selectedFine.state_id);
            CostTextBox.Text = _selectedFine.cost.ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DriverComboBox.SelectedItem == null || CarComboBox.SelectedItem == null || StateComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (водитель, автомобиль, статус штрафа).",
                                "Ошибка обязательных полей", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                var fineToUpdate = context.Fines.FirstOrDefault(f => f.id == _selectedFine.id);

                if (fineToUpdate != null)
                {
                    fineToUpdate.driver_id = ((Drivers)DriverComboBox.SelectedItem)?.id ?? 0;
                    fineToUpdate.car_id = ((Cars)CarComboBox.SelectedItem)?.id ?? 0;
                    fineToUpdate.state_id = ((StateOfFines)StateComboBox.SelectedItem)?.id ?? 0;
                    fineToUpdate.cost = cost;

                    context.SaveChanges();
                    MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

