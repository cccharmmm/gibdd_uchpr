using gibdd_uchpr.model;
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

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для FinesWindow.xaml
    /// </summary>
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
