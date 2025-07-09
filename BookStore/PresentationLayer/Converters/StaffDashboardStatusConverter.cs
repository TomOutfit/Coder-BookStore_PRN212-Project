using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PresentationLayer.Converters
{
    public class StaffDashboardStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return "Loading data...";
            }

            int booksInStock = (int)values[0];
            int pendingOrders = (int)values[1];
            return $"Có {booksInStock} sách trong kho, {pendingOrders} đơn hàng đang chờ xử lý";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}