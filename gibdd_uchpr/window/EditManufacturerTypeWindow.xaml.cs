using gibdd_uchpr.model;
using System.Linq;
using System.Windows;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для EditManufacturerTypeWindow.xaml
    /// </summary>
    public partial class EditManufacturerTypeWindow : Window
    {
        private ManufacturerType _manufacturertype;
        public EditManufacturerTypeWindow(ManufacturerType selectedManufacturer)
        {
            InitializeComponent();
            _manufacturertype = selectedManufacturer;
            NameTextBox.Text = _manufacturertype.name;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;

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
                var manufacturerToUpdate = context.ManufacturerType.FirstOrDefault(c => c.id == _manufacturertype.id);

                if (manufacturerToUpdate != null)
                {
                    manufacturerToUpdate.name = NameTextBox.Text;
                    context.SaveChanges();
                }
            }

            MessageBox.Show("Изменения сохранены!");
            Close();
        }
    }

}
