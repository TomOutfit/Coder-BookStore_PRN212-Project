using System;
using System.Globalization;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class BookWiseCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount && parameter is string currency)
            {
                return $"{amount:N0} {currency}";
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BookWiseCurrencyConverter does not support two-way conversion.");
        }
    }
} 