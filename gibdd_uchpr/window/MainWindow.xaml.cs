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
            Authorization auth = new Authorization();
            auth.Show();
            this.Close();
        }
    }
}
