using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using PresentationLayer.Commands;
using Entities;
using System.Threading.Tasks;
using System;

namespace PresentationLayer.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private User? _currentUser;
        private string _password;

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Username));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProfileViewModel(IUserService userService)
        {
            _userService = userService;
            SaveCommand = new BookWiseRelayCommand(_ => Save());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
            LoadCurrentUser();
        }

        private async void LoadCurrentUser()
        {
            if (System.Windows.Application.Current is PresentationLayer.App app && app.CurrentUser != null)
            {
                // Lấy lại user đầy đủ từ DB
                var user = await _userService.GetUserByIdAsync(app.CurrentUser.Id);
                if (user != null)
                {
                    CurrentUser = user;
                    OnPropertyChanged(nameof(FullName));
                    OnPropertyChanged(nameof(Email));
                    OnPropertyChanged(nameof(PhoneNumber));
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string FullName
        {
            get => CurrentUser?.FullName ?? string.Empty;
        }
        public string Email
        {
            get => CurrentUser?.Email ?? string.Empty;
            set
            {
                if (CurrentUser != null && CurrentUser.Email != value)
                {
                    CurrentUser.Email = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PhoneNumber
        {
            get => CurrentUser?.PhoneNumber ?? string.Empty;
            set
            {
                if (CurrentUser != null && CurrentUser.PhoneNumber != value)
                {
                    CurrentUser.PhoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Username
        {
            get => CurrentUser?.Username ?? string.Empty;
            set
            {
                if (CurrentUser != null && CurrentUser.Username != value)
                {
                    CurrentUser.Username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FirstName
        {
            get => CurrentUser?.FirstName ?? string.Empty;
            set
            {
                if (CurrentUser != null && CurrentUser.FirstName != value)
                {
                    CurrentUser.FirstName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string LastName
        {
            get => CurrentUser?.LastName ?? string.Empty;
            set
            {
                if (CurrentUser != null && CurrentUser.LastName != value)
                {
                    CurrentUser.LastName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private async void Save()
        {
            try
            {
                if (CurrentUser == null) return;
                if (!string.IsNullOrEmpty(Password))
                {
                    CurrentUser.Password = Password;
                }
                var success = await _userService.UpdateUserAsync(CurrentUser);
                if (success)
                {
                    // Lấy lại user mới nhất từ DB để đồng bộ UI
                    var updatedUser = await _userService.GetUserByIdAsync(CurrentUser.Id);
                    if (updatedUser != null)
                    {
                        CurrentUser = updatedUser;
                        OnPropertyChanged(nameof(FullName));
                        OnPropertyChanged(nameof(Email));
                        OnPropertyChanged(nameof(PhoneNumber));
                        OnPropertyChanged(nameof(Username));
                        OnPropertyChanged(nameof(FirstName));
                        OnPropertyChanged(nameof(LastName));
                    }
                    Password = string.Empty;
                    System.Windows.MessageBox.Show("Cập nhật thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    DataChangeNotifier.NotifyDataChanged();
                }
                else
                {
                    System.Windows.MessageBox.Show("Cập nhật thất bại!", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            // TODO: Implement cancel logic
            LoadCurrentUser(); // Reload original data
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                // Implementation for loading user profile
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                // Handle exception
            }
        }

        public void LoadUser(User user)
        {
            CurrentUser = user;
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 