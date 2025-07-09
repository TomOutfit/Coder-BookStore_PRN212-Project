using System;
using System.Globalization;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    public class BookWiseBooleanToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "Hoạt động" : "Đã khóa";
            return "Không xác định";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return s == "Hoạt động";
            return false;
        }
    }
} 