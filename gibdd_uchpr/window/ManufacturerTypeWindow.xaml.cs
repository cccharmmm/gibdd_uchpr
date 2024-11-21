using gibdd_uchpr.model;
using System.Windows;


namespace gibdd_uchpr.window
{
    public partial class ManufacturerTypeWindow : Window
    {
        public ManufacturerTypeWindow()
        {
            InitializeComponent();
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
    }

}
