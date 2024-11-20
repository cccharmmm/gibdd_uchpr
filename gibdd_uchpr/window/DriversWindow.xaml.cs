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
using System.IO;
using Microsoft.Win32;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для DriversWindow.xaml
    /// </summary>
    public partial class DriversWindow : Window
    {
        public DriversWindow()
        {
            InitializeComponent();
            Loaded += Driver_Loaded;
            LoadCompanyJob();
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
        //private void photoButton(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.* ";
        //    if ((bool)openFileDialog.ShowDialog())
        //    {
        //        try
        //        {
        //            this.im = File.ReadAllBytes(openFileDialog.FileName);
        //            lb1.Visibility = Visibility.Visible;
        //        }
        //        catch
        //        {
        //            lb1.Visibility = Visibility.Visible;
        //            lb1.Content = "Ошибка";
        //        }
        //    }
        //}
    }
}
