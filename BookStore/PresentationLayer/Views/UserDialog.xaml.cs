using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer.Views
{
    public partial class UserDialog : Window
    {
        public UserDialog(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UserDialogViewModel vm)
                vm.Password = PasswordBox.Password;
        }
    }
} 