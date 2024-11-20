using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using gibdd_uchpr.model;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для LicencesWindow.xaml
    /// </summary>
    public partial class LicencesWindow : Window
    {
        public LicencesWindow()
        {
            InitializeComponent();
            Loaded += License_Loaded;
        }
        private void UpdateLicenseList()
        {
            using (var context = new gibddEntities())
            {
                var fines = context.Licenses
                    .Include(l => l.Drivers)
                    .ToList();

                LicenseListBox.ItemsSource = fines;
            }
        }

        private void License_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLicenseList();
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
        private void finesButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы перешли на окно «Штрафы»");
            FinesWindow finesWindow = new FinesWindow();
            finesWindow.Show();
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
