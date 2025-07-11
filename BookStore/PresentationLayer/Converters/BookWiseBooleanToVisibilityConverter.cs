using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BookWiseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BookWiseBooleanToVisibilityConverter does not support two-way conversion.");
        }
    }
} 