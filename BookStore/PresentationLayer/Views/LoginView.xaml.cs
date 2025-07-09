using System.Windows;
using BusinessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PresentationLayer.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var app = Application.Current as App;
            var userService = app?.ServiceProvider.GetRequiredService<IUserService>();
            DataContext = new ViewModels.LoginViewModel(this, userService!);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}