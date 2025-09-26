using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SolarOrderQuiz.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool CollapseWhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag && flag)
            {
                return Visibility.Visible;
            }

            return CollapseWhenFalse ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
