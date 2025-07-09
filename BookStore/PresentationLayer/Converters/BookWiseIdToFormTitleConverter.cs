using System;
using System.Globalization;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class BookWiseIdToFormTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && parameter is string entityType)
            {
                return id > 0 ? $"Cập nhật {entityType} (ID: {id})" : $"Thêm mới {entityType}";
            }
            return "Form";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BookWiseIdToFormTitleConverter does not support two-way conversion.");
        }
    }
} 