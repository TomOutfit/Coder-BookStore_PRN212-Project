using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class BookWiseRoleVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role && parameter is string allowedRoles)
            {
                var roles = allowedRoles.Split(',').Select(r => r.Trim());
                return roles.Contains(role) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BookWiseRoleVisibilityConverter does not support two-way conversion.");
        }
    }
} 