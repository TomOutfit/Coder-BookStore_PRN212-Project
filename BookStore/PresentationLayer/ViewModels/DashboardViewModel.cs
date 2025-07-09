using System.ComponentModel;
using System.Runtime.CompilerServices;
using BusinessLayer.Services;
using System.Windows;
using System.Windows.Input;
using PresentationLayer.Commands;

namespace PresentationLayer.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCategories { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand GoToBooksCommand { get; }
        public ICommand GoToUsersCommand { get; }
        public ICommand GoToOrdersCommand { get; }
        public ICommand GoToCategoriesCommand { get; }
        public event Action<string>? RequestNavigate;
        public DashboardViewModel(ICategoryService categoryService, IBookService bookService, IUserService userService, IOrderService orderService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _userService = userService;
            _orderService = orderService;
            LoadStats();
            DashboardStatsChanged += (s, e) => ReloadStats();

            GoToBooksCommand = new BookWiseRelayCommand(_ => GoToBooks());
            GoToUsersCommand = new BookWiseRelayCommand(_ => GoToUsers());
            GoToOrdersCommand = new BookWiseRelayCommand(_ => GoToOrders());
            GoToCategoriesCommand = new BookWiseRelayCommand(_ => GoToCategories());
        }
        public static event EventHandler? DashboardStatsChanged;
        public static void NotifyStatsChanged() => DashboardStatsChanged?.Invoke(null, EventArgs.Empty);
        public void ReloadStats() => LoadStats();
        private async void LoadStats()
        {
            TotalBooks = await _bookService.CountBooksAsync();
            await Task.Yield();
            TotalUsers = await _userService.CountUsersAsync();
            await Task.Yield();
            TotalOrders = await _orderService.CountOrdersAsync();
            await Task.Yield();
            TotalRevenue = await _orderService.CalculateTotalRevenueAsync();
            await Task.Yield();
            TotalCategories = await _categoryService.CountCategoriesAsync();
            OnPropertyChanged(nameof(TotalBooks));
            OnPropertyChanged(nameof(TotalUsers));
            OnPropertyChanged(nameof(TotalOrders));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalCategories));
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void GoToBooks()
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Books");
        }
        private void GoToUsers()
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Users");
        }
        private void GoToOrders()
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Orders");
        }
        private void GoToCategories()
        {
            if (!IsAdmin())
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Categories");
        }
        private bool IsAdmin()
        {
            var app = Application.Current as PresentationLayer.App;
            return app?.CurrentUser?.Role == "Admin";
        }
    }
} 