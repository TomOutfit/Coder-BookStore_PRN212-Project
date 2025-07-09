using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BusinessLayer.Services;
using Microsoft.Extensions.DependencyInjection;
using PresentationLayer; // Đảm bảo using đúng namespace MainWindow

namespace PresentationLayer.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private readonly Window _loginWindow;
        private readonly IUserService _userService;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; }

        public LoginViewModel(Window loginWindow, IUserService userService)
        {
            _loginWindow = loginWindow ?? throw new ArgumentNullException(nameof(loginWindow));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            ErrorMessage = string.Empty;
            try
            {
                var user = await _userService.AuthenticateAsync(Username, Password);
                if (user == null)
                {
                    ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return;
                }
                // Lấy lại user đầy đủ từ DB (bao gồm RoleNavigation)
                var fullUser = await _userService.GetUserByIdAsync(user.Id);
                if (System.Windows.Application.Current is PresentationLayer.App app)
                {
                    app.CurrentUser = fullUser;
                }
                // Đóng LoginView
                foreach (var w in System.Windows.Application.Current.Windows)
                {
                    if (w is PresentationLayer.Views.LoginView login && login.DataContext == this)
                    {
                        login.DialogResult = true;
                        login.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand đơn giản cho WPF
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
} 