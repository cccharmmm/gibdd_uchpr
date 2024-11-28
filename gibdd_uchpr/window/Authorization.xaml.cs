using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace gibdd_uchpr.window
{
    public partial class Authorization : Window
    {
        public int failedAttempts = 0;
        public DateTime? lockEndTime = null;
        public DispatcherTimer inactivityTimer;
        public DateTime lastActivityTime;

        public TextBox LoginTextBox => loginTextBox;  
        public TextBox PasswordBox => passwordBox;

        private readonly gibddEntities _context;
        public Authorization(gibddEntities context)
        {
            System.Windows.Application.ResourceAssembly = typeof(Authorization).Assembly;
            InitializeComponent();
            _context = context;
            inactivityTimer = new DispatcherTimer();
            inactivityTimer.Interval = TimeSpan.FromMinutes(1);
            inactivityTimer.Tick += InactivityTimer_Tick;
            lastActivityTime = DateTime.Now;
        }

        public void loginButton(object sender, RoutedEventArgs e)
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

            using (var context = new gibddEntities())
            {
                var user = context.Users
                    .FirstOrDefault(u => u.log_in == login && u.passwword == password);

                if (user == null)
                {
                    failedAttempts++;
                    MessageBox.Show("Неверный логин или пароль!");

                    if (failedAttempts >= 3)
                    {
                        lockEndTime = DateTime.Now.AddMinutes(1);
                        MessageBox.Show("Слишком много неудачных попыток. Попробуйте позже.");
                    }

                    return;
                }

                if (user.log_in == "inspector" && user.passwword == "inspector")
                {
                    failedAttempts = 0;
                    lockEndTime = null;
                    MessageBox.Show("Добро пожаловать, Инспектор!");
                    DriversWindow driversWindow = new DriversWindow();
                    driversWindow.Show();
                    this.Close();
                }
                else if (user.log_in == "admin" && user.passwword == "admin")
                {
                    failedAttempts = 0;
                    lockEndTime = null;
                    MessageBox.Show("Добро пожаловать, Администратор!");
                    ManufacturerTypeWindow adminWindow = new ManufacturerTypeWindow();
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неизвестная роль.");
                }
            }
        }

        public void InactivityTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - lastActivityTime > TimeSpan.FromMinutes(1))
            {
                MessageBox.Show("Приложение не использовалось более минуты. Вы будете перенаправлены на страницу авторизации.");
                this.Close();
            }
        }

        public void Window_Activated(object sender, EventArgs e)
        {
            lastActivityTime = DateTime.Now;
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inactivityTimer.Start();
        }

        public void backButton(object sender, RoutedEventArgs e)
        {
            MainWindow mW = new MainWindow();
            mW.Show();
            this.Close();
        }
    }
}
