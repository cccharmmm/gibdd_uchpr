using System;
using System.Windows;
using System.Windows.Data;

namespace gibdd_uchpr.classes
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fileName = value as string;
            if (fileName != null)
            {
                string ImagesFolderPath = "C:\\Users\\almaz\\source\\repos\\gibdd_uchpr\\gibdd_uchpr\\images\\"; // Путь к папке с изображениями
                string imagePath = ImagesFolderPath + fileName;

                // Проверка на корректность пути
                if (System.IO.File.Exists(imagePath))
                {
                    return imagePath;  // Возвращаем путь, если файл существует
                }
                else
                {
                    // Если изображение не найдено, выводим сообщение
                    if (Application.Current.Dispatcher.CheckAccess())  // Проверка, находится ли в главном потоке
                    {
                        MessageBox.Show($"Изображение не найдено по пути: {imagePath}");
                    }
                    else
                    {
                        // Если мы не в главном потоке, используем Invoke для работы с UI
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Изображение не найдено по пути: {imagePath}");
                        });
                    }
                    return null;  // Если изображения нет, возвращаем null
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
