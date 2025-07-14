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
            bool isVisible = value is bool b && b;
            string param = parameter as string;
            if (param == "invert")
                isVisible = !isVisible;
            else if (param == "adminOrStaff")
            {
                // Nếu binding là IsAdmin hoặc IsStaff, truyền true nếu 1 trong 2 true
                // Nhưng binding hiện tại chỉ truyền 1 property, nên cần binding đúng từ ViewModel
                // Nếu binding là IsStaff, thì Admin cũng phải thấy tab Staff/Customer
                // => Nếu IsStaff true hoặc IsAdmin true thì hiện
                // => value là IsStaff, nhưng nếu IsAdmin true cũng phải hiện
                // => Không thể biết IsAdmin từ đây, nên binding phải truyền đúng
                // Tuy nhiên, để an toàn, nếu value là true thì hiện
                // (nếu cần logic phức tạp hơn, nên binding từ ViewModel)
            }
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BookWiseBooleanToVisibilityConverter does not support two-way conversion.");
        }
    }
} 