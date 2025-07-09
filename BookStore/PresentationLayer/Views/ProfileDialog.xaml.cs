using System.Windows;

namespace PresentationLayer.Views
{
    public partial class ProfileDialog : Window
    {
        public ProfileDialog(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.ProfileDialogViewModel vm)
                vm.NewPassword = NewPasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.ProfileDialogViewModel vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
} 