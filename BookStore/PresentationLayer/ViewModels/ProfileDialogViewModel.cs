using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using PresentationLayer.Commands;

namespace PresentationLayer.ViewModels
{
    public class ProfileDialogViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        public User User { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string DialogTitle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ProfileDialogViewModel(IUserService userService, User user, string title = "Cập nhật thông tin cá nhân")
        {
            _userService = userService;
            DialogTitle = title;
            User = new User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive
            };
            SaveCommand = new BookWiseRelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
        }
        private bool CanSave()
        {
            if (!string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmPassword))
                return !string.IsNullOrWhiteSpace(NewPassword) && NewPassword == ConfirmPassword;
            return !string.IsNullOrWhiteSpace(User.FirstName) && !string.IsNullOrWhiteSpace(User.LastName) && !string.IsNullOrWhiteSpace(User.Email);
        }
        private async void Save()
        {
            ErrorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(NewPassword))
                    User.Password = NewPassword;
                await _userService.UpdateUserAsync(User);
                CloseDialog(true);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        private void Cancel() => CloseDialog(false);
        private void CloseDialog(bool result)
        {
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is PresentationLayer.Views.ProfileDialog dialog && dialog.DataContext == this)
                {
                    dialog.DialogResult = result;
                    dialog.Close();
                    break;
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 