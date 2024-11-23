using gibdd_uchpr.model;
using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;


namespace gibdd_uchpr.window
{
    public partial class HistoryOfStatusWindow : Window
    {
        private int licenseId;
        public HistoryOfStatusWindow(int licenseId)
        {
            InitializeComponent();
            this.licenseId = licenseId; 
            LoadHistory();
        }
        private void LoadHistory()
        {
            using (var context = new gibddEntities())
            {
                var history = context.HistoryOfStatus
                                     .Where(h => h.license_id == licenseId)
                                     .ToList();

                HistoryOfStatusListBox.ItemsSource = history;
            }
        }
        private void EditSelectedHos(object sender, RoutedEventArgs e)
        {
            if (HistoryOfStatusListBox.SelectedItem is HistoryOfStatus selectedHistory)
            {
                var editHosWindow = new EditHistoryWindow(selectedHistory);
                editHosWindow.ShowDialog();
                UpdateHosList();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.");
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string date_of_chanfe = DateOfChangeTextBox.Text;
            string comment = CommentTextBox.Text;
            string status = (StatusComboBox.SelectedItem as StateOfLicenses)?.name;
            string license = (LComboBox.SelectedItem as Licenses)?.license_series;

            if (string.IsNullOrWhiteSpace(date_of_chanfe) && string.IsNullOrWhiteSpace(comment) &&
                string.IsNullOrWhiteSpace(status) && string.IsNullOrWhiteSpace(license))
            {
                MessageBox.Show("Заполните хотя бы одно поле для поиска.");
                return;
            }

            using (var context = new gibddEntities())
            {
                var query = context.HistoryOfStatus.Include(d => d.Licenses)
                                                    .Include(d => d.StateOfLicenses)
                                                  .AsQueryable();

                if (!string.IsNullOrWhiteSpace(date_of_chanfe))
                {
                    query = query.Where(d => d.date_of_change.Contains(date_of_chanfe));
                }

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    query = query.Where(d => d.comment.Contains(comment));
                }
                if (!string.IsNullOrWhiteSpace(status))
                {
                    var stateId = context.StateOfLicenses
                                          .Where(d => d.name.Contains(status))
                                          .Select(d => d.id)
                                          .FirstOrDefault();

                    if (stateId > 0)
                    {
                        query = query.Where(d => d.status_id == stateId);
                    }
                    else
                    {
                        MessageBox.Show("Статус не найден.");
                        return;
                    }
                }
                if (!string.IsNullOrWhiteSpace(license))
                {
                    var licenseId = context.Licenses
                                          .Where(d => d.license_series.Contains(license))
                                          .Select(d => d.id)
                                          .FirstOrDefault();

                    if (licenseId > 0)
                    {
                        query = query.Where(d => d.license_id == licenseId);
                    }
                    else
                    {
                        MessageBox.Show("ВУ не найден.");
                        return;
                    }
                }

                var searchResults = query.ToList();
                HistoryOfStatusListBox.ItemsSource = searchResults;
            }
        }
        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryOfStatusListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedHos = HistoryOfStatusListBox.SelectedItem as HistoryOfStatus;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить запись от: {selectedHos.date_of_change}?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new gibddEntities())
                {
                    try
                    {
                        var noteToDelete = context.HistoryOfStatus.Find(selectedHos.id);

                        if (noteToDelete != null)
                        {
                            context.HistoryOfStatus.Remove(noteToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateHosList();
                        }
                        else
                        {
                            MessageBox.Show("Запись не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void CreateNote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DateOfChangeTextBox.Text) ||
                string.IsNullOrWhiteSpace(CommentTextBox.Text) ||
                StatusComboBox.SelectedItem == null ||
                LComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.",
                                "Ошибка заполненности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParseExact(DateOfChangeTextBox.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime licenseDate))
            {
                MessageBox.Show("Пожалуйста, введите дату выдачи в формате dd.MM.yyyy.",
                                "Ошибка формата даты", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new gibddEntities())
            {
                var stateId = (StatusComboBox.SelectedItem as StateOfLicenses)?.id ?? 0;
                var licenseId = (LComboBox.SelectedItem as Licenses)?.id ?? 0;
                try
                {
                    var newHistoryOfStatus = new HistoryOfStatus
                    {
                        date_of_change = DateOfChangeTextBox.Text,
                        comment = CommentTextBox.Text,
                        status_id = stateId,
                        license_id = licenseId
                    };

                    context.HistoryOfStatus.Add(newHistoryOfStatus);
                    context.SaveChanges();
                    UpdateHosList();
                    MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}\n{ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void UpdateHosList()
        {
            using (var context = new gibddEntities())
            {
                var historyOfStatuses = context.HistoryOfStatus
                    .Include(c => c.StateOfLicenses)
                    .Include(c => c.Licenses)
                    .ToList();

                HistoryOfStatusListBox.ItemsSource = historyOfStatuses;
            }
        }
        private void LoadStateOfLicences()
        {
            using (var context = new gibddEntities())
            {
                var types = context.StateOfLicenses.ToList();
                StatusComboBox.ItemsSource = types;
            }
        }
        private void LoadLicences()
        {
            using (var context = new gibddEntities())
            {
                var types = context.Licenses.ToList();
                LComboBox.ItemsSource = types;
            }
        }
        private void Hos_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateHosList();
        }
        public void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateHosList();
        }

    }
}
