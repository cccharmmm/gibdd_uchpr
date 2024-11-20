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
using System.Windows.Threading;

namespace gibdd_uchpr.window
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        private int failedAttempts = 0;
        private DateTime? lockEndTime = null; 
        private DispatcherTimer inactivityTimer; 
        private DateTime lastActivityTime;
        public Authorization()
        {
            InitializeComponent();
            inactivityTimer = new DispatcherTimer();
            inactivityTimer.Interval = TimeSpan.FromMinutes(1);
            inactivityTimer.Tick += InactivityTimer_Tick;
            lastActivityTime = DateTime.Now;
        }
        private void loginButton(object sender, RoutedEventArgs e)
        {
            if (lockEndTime.HasValue && DateTime.Now < lockEndTime.Value)
            {
                MessageBox.Show("Вы заблокированы, попробуйте позже.");
                return;
            }

            string login = LoginTextBox.Text;
            string password = PasswordBox.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (login == "inspector" && password == "inspector")
            {
                failedAttempts = 0; 
                lockEndTime = null;
                MessageBox.Show("Добро пожаловать! Авторизация прошла успешно.");
                DriversWindow driversWindow = new DriversWindow();
                driversWindow.Show();
                this.Close(); 
            }
            else
            {
                failedAttempts++;
                MessageBox.Show("Неверный логин или пароль!");

                if (failedAttempts >= 3)
                {
                    lockEndTime = DateTime.Now.AddMinutes(1); 
                    MessageBox.Show("Слишком много неудачных попыток. Попробуйте позже.");
                }
            }
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - lastActivityTime > TimeSpan.FromMinutes(1))
            {
                MessageBox.Show("Приложение не использовалось более минуты. Вы будете перенаправлены на страницу авторизации.");
                this.Close();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            lastActivityTime = DateTime.Now;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inactivityTimer.Start();
        }
        private void backButton(object sender, RoutedEventArgs e)
        {
            MainWindow mW = new MainWindow();
            mW.Show();
            this.Close();
        }
    }
}
