using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Логика взаимодействия для LicencesWindow.xaml
    /// </summary>
    public partial class LicencesWindow : Window
    {
        public LicencesWindow()
        {
            InitializeComponent();
        }
        private void driversButton(object sender, RoutedEventArgs e)
        {
            DriversWindow driversWindow = new DriversWindow();
            driversWindow.Show();
            this.Close();
        }
        private void carsButton(object sender, RoutedEventArgs e)
        {
            CarsWindow carsWindow = new CarsWindow();
            carsWindow.Show();
            this.Close();
        }
        private void finesButton(object sender, RoutedEventArgs e)
        {
            FinesWindow finesWindow = new FinesWindow();
            finesWindow.Show();
            this.Close();
        }
        private void exitButton(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
