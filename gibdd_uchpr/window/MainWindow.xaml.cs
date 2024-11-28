using gibdd_uchpr.model;
using System.Windows;

namespace gibdd_uchpr.window
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добро пожаловать!");
            gibddEntities context = new gibddEntities();
            Authorization auth = new Authorization(context);
            auth.Show();
            this.Close();
        }
    }
}
