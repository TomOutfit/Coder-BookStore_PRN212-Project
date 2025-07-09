using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using PresentationLayer.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class UserDialogViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public User User { get; set; }
        public ObservableCollection<Role> Roles { get; set; } = new();
        private Role? _selectedRole;
        public Role? SelectedRole { get => _selectedRole; set { _selectedRole = value; User.Role = value?.Name ?? "User"; OnPropertyChanged(); } }
        public ObservableCollection<string> StatusList { get; set; }
        public string SelectedStatus
        {
            get => User.IsActive ? "Active" : "Inactive";
            set { User.IsActive = value == "Active"; OnPropertyChanged(); }
        }
        public string Password { get; set; } = string.Empty;
        public string DialogTitle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public UserDialogViewModel(IUserService userService, IRoleService roleService, User? user = null, string title = "Thêm người dùng")
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            DialogTitle = title;
            User = user != null ? new User
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
            } : new User();
            SaveCommand = new BookWiseRelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
            StatusList = new ObservableCollection<string>(new[] { "Active", "Inactive" });
            LoadRoles(); // Luôn load vai trò khi mở dialog
        }
        private async void LoadRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            Roles = new ObservableCollection<Role>(roles);
            if (!string.IsNullOrWhiteSpace(User.Role))
                SelectedRole = Roles.FirstOrDefault(r => r.Name == User.Role);
            OnPropertyChanged(nameof(Roles));
            OnPropertyChanged(nameof(SelectedRole));
        }
        private bool CanSave()
        {
            if (User.Id == 0)
                return !string.IsNullOrWhiteSpace(User.Username) && !string.IsNullOrWhiteSpace(User.Email) && SelectedRole != null && !string.IsNullOrWhiteSpace(Password);
            return !string.IsNullOrWhiteSpace(User.Username) && !string.IsNullOrWhiteSpace(User.Email) && SelectedRole != null;
        }
        private async void Save()
        {
            ErrorMessage = string.Empty;
            try
            {
                User.Role = SelectedRole?.Name ?? "User";
                User.IsActive = SelectedStatus == "Active";
                if (!string.IsNullOrWhiteSpace(Password))
                    User.Password = Password;
                if (User.Id == 0)
                    await _userService.AddUserAsync(User);
                else
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
                if (w is PresentationLayer.Views.UserDialog dialog && dialog.DataContext == this)
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