using System.Collections.ObjectModel;
using System.Windows.Input;
using BusinessLayer;
using Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PresentationLayer.Commands;
using System.Threading.Tasks;
using BusinessLayer.Services;
using PresentationLayer.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;

namespace PresentationLayer.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public ObservableCollection<User> Users { get; set; } = new();
        private User? _selectedUser;
        public User? SelectedUser { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); SearchUsers(); } }
        private int _currentPage = 1;
        public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); LoadUsers(); } }
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public ObservableCollection<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50, 100 };
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public UserManagementViewModel(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
            AddUserCommand = new BookWiseRelayCommand(_ => AddUser());
            EditUserCommand = new BookWiseRelayCommand(u => EditUser(u as User), u => u is User);
            DeleteUserCommand = new BookWiseRelayCommand(u => DeleteUser(u as User), u => u is User);
            NextPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage < TotalPages) { CurrentPage++; } });
            PrevPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage > 1) { CurrentPage--; } });
            SearchCommand = new BookWiseRelayCommand(_ => SearchUsers());
            RefreshCommand = new BookWiseRelayCommand(_ => RefreshUsers());
            _ = LoadUsers();
            DataChangeNotifier.DataChanged += () => LoadUsers();
        }
        private async Task LoadUsers()
        {
            var result = await _userService.GetAllUsersAsync(CurrentPage, PageSize, SearchText);
            var total = await _userService.CountUsersAsync(SearchText);
            Users.Clear();
            foreach (var u in result) Users.Add(u);
            TotalPages = (total + PageSize - 1) / PageSize;
            OnPropertyChanged(nameof(TotalPages));
        }
        private async void SearchUsers() { CurrentPage = 1; await LoadUsers(); }
        private void AddUser()
        {
            try
            {
                var vm = new UserDialogViewModel(_userService, _roleService, null, "Thêm người dùng");
                var dialog = new UserDialog(vm);
                if (dialog.ShowDialog() == true)
                {
                    LoadUsers();
                    MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dialog: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditUser()
        {
            if (SelectedUser != null)
                EditUser(SelectedUser);
        }
        private void EditUser(User? user)
        {
            if (user == null) return;
            try
            {
                var vm = new UserDialogViewModel(_userService, _roleService, user, "Sửa người dùng");
                var dialog = new UserDialog(vm);
                if (dialog.ShowDialog() == true)
                {
                    using var _ = LoadUsers();
                    MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dialog: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteUser()
        {
            if (SelectedUser != null)
                DeleteUser(SelectedUser);
        }
        private async void DeleteUser(User? user)
        {
            if (user == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn xóa người dùng '{user.Username}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    await _userService.DeleteUserAsync(user.Id);
                    _ = LoadUsers();
                    PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                    MessageBox.Show("Xóa người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataChangeNotifier.NotifyDataChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void RefreshUsers()
        {
            try
            {
                await LoadUsers();
                MessageBox.Show("Làm mới danh sách người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowSnackbar(string message)
        {
            // TODO: Hiện snackbar/toast UI, tạm thời dùng MessageBox
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 