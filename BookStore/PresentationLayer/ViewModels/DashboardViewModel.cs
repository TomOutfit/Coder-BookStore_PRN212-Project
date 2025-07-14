using System.ComponentModel;
using System.Runtime.CompilerServices;
using BusinessLayer.Services;
using System.Windows;
using System.Windows.Input;
using PresentationLayer.Commands;
using Entities;
using System.Windows.Threading;
using System;
using System.Threading.Tasks;

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
        private User? CurrentUser => (System.Windows.Application.Current as PresentationLayer.App)?.CurrentUser;
        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsStaff => CurrentUser?.Role == "Staff";
        public bool IsCustomer => CurrentUser?.Role == "Customer";
        public bool IsAdminOrStaff => IsAdmin || IsStaff;
        public bool IsStaffOrCustomer => IsStaff || IsCustomer;

        public int BookCount { get; set; }
        public int OrderCount { get; set; }
        public int MyOrderCount { get; set; }
        public int MyBookCount { get; set; }

        public ICommand GoToBooksCommand { get; set; }
        public ICommand GoToOrdersCommand { get; set; }
        public ICommand GoToMyOrdersCommand { get; set; }
        public ICommand GoToProfileCommand { get; set; }
        public event Action<string>? RequestNavigate;
        private DispatcherTimer _timer;
        public DashboardViewModel(ICategoryService categoryService, IBookService bookService, IUserService userService, IOrderService orderService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _userService = userService;
            _orderService = orderService;
            LoadStats();
            DashboardStatsChanged += (s, e) => ReloadStats();

            GoToBooksCommand = new BookWiseRelayCommand(_ => GoToBooks());
            GoToOrdersCommand = new BookWiseRelayCommand(_ => GoToOrders());
            GoToMyOrdersCommand = new BookWiseRelayCommand(_ => GoToMyOrders());
            GoToProfileCommand = new BookWiseRelayCommand(_ => GoToProfile());

            // Realtime update every 5 seconds
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += (s, e) => LoadStats();
            _timer.Start();
        }
        public static event EventHandler? DashboardStatsChanged;
        public static void NotifyStatsChanged() => DashboardStatsChanged?.Invoke(null, EventArgs.Empty);
        public void ReloadStats() => LoadStats();
        private async void LoadStats()
        {
            BookCount = await _bookService.CountBooksAsync();
            OrderCount = await _orderService.CountOrdersAsync();
            MyOrderCount = CurrentUser != null ? await _orderService.CountOrdersByUserAsync(CurrentUser.Id) : 0;
            MyBookCount = CurrentUser != null ? await _bookService.CountBooksByUserAsync(CurrentUser.Id) : 0;
            TotalBooks = BookCount;
            TotalOrders = OrderCount;
            TotalUsers = await _userService.CountUsersAsync();
            TotalRevenue = await _orderService.CalculateTotalRevenueAsync();
            TotalCategories = await _categoryService.CountCategoriesAsync();
            OnPropertyChanged(nameof(BookCount));
            OnPropertyChanged(nameof(OrderCount));
            OnPropertyChanged(nameof(MyOrderCount));
            OnPropertyChanged(nameof(MyBookCount));
            OnPropertyChanged(nameof(TotalBooks));
            OnPropertyChanged(nameof(TotalUsers));
            OnPropertyChanged(nameof(TotalOrders));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalCategories));
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void GoToBooks()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Books");
        }
        private void GoToOrders()
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Chỉ Admin mới được truy cập chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RequestNavigate?.Invoke("Orders");
        }
        private void GoToMyOrders() { /* TODO: Implement navigation to My Orders */ }
        private void GoToProfile() { /* TODO: Implement navigation to Profile */ }
    }
} 