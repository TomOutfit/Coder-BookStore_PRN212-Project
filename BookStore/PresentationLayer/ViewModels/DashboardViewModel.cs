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
        #region Services
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        #endregion

        #region Properties
        // Admin Dashboard Stats
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCategories { get; set; }

        // Staff Dashboard Stats
        public int BookCount { get; set; }
        public int OrderCount { get; set; }

        // Customer Dashboard Stats
        public int MyOrderCount { get; set; }
        public int MyBookCount { get; set; }

        // User Role Properties
        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsStaff => CurrentUser?.Role == "Staff";
        public bool IsCustomer => CurrentUser?.Role == "Customer" || CurrentUser?.Role == "User";
        public bool IsAdminOrStaff => IsAdmin || IsStaff;
        public bool IsStaffOrCustomer => IsStaff || IsCustomer;

        // Current User
        private User? CurrentUser => (Application.Current as App)?.CurrentUser;
        #endregion

        #region Commands
        public ICommand GoToBooksCommand { get; }
        public ICommand GoToOrdersCommand { get; }
        public ICommand GoToMyOrdersCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand RefreshStatsCommand { get; }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? RequestNavigate;
        public static event EventHandler? DashboardStatsChanged;
        #endregion

        #region Private Fields
        private readonly DispatcherTimer _refreshTimer;
        private bool _isLoading = false;
        #endregion

        #region Constructor
        public DashboardViewModel(
            ICategoryService categoryService, 
            IBookService bookService, 
            IUserService userService, 
            IOrderService orderService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            // Initialize Commands
            GoToBooksCommand = new BookWiseRelayCommand(_ => NavigateToBooks());
            GoToOrdersCommand = new BookWiseRelayCommand(_ => NavigateToOrders());
            GoToMyOrdersCommand = new BookWiseRelayCommand(_ => NavigateToMyOrders());
            GoToProfileCommand = new BookWiseRelayCommand(_ => NavigateToProfile());
            RefreshStatsCommand = new BookWiseRelayCommand(_ => RefreshStatsAsync());

            // Initialize Timer
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Refresh every 10 seconds
            };
            _refreshTimer.Tick += async (s, e) => await LoadStatsAsync();

            // Subscribe to stats change events
            DashboardStatsChanged += (s, e) => _ = RefreshStatsAsync();

            // Start timer and load initial data
            _refreshTimer.Start();
            _ = LoadStatsAsync();
        }
        #endregion

        #region Public Methods
        public void ReloadStats() => _ = RefreshStatsAsync();

        public static void NotifyStatsChanged() => DashboardStatsChanged?.Invoke(null, EventArgs.Empty);
        #endregion

        #region Private Methods - Navigation
        private void NavigateToBooks()
        {
            if (!IsAdmin)
            {
                ShowAccessDeniedMessage("Chỉ Admin mới được truy cập chức năng này!");
                return;
            }
            RequestNavigate?.Invoke("Books");
        }

        private void NavigateToOrders()
        {
            if (!IsAdmin)
            {
                ShowAccessDeniedMessage("Chỉ Admin mới được truy cập chức năng này!");
                return;
            }
            RequestNavigate?.Invoke("Orders");
        }

        private void NavigateToMyOrders()
        {
            if (!IsCustomer)
            {
                ShowAccessDeniedMessage("Chỉ khách hàng mới được xem đơn hàng của mình!");
                return;
            }
            RequestNavigate?.Invoke("MyOrders");
        }

        private void NavigateToProfile()
        {
            RequestNavigate?.Invoke("Profile");
        }

        private void ShowAccessDeniedMessage(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion

        #region Private Methods - Data Loading
        private async Task LoadStatsAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                // Load basic stats
                var bookCountTask = _bookService.CountBooksAsync();
                var orderCountTask = _orderService.CountOrdersAsync();
                var userCountTask = _userService.CountUsersAsync();
                var revenueTask = _orderService.CalculateTotalRevenueAsync();
                var categoryCountTask = _categoryService.CountCategoriesAsync();

                // Wait for all tasks to complete
                await Task.WhenAll(bookCountTask, orderCountTask, userCountTask, revenueTask, categoryCountTask);

                // Update properties
                TotalBooks = BookCount = await bookCountTask;
                TotalOrders = OrderCount = await orderCountTask;
                TotalUsers = await userCountTask;
                TotalRevenue = await revenueTask;
                TotalCategories = await categoryCountTask;

                // Load user-specific stats if user is logged in
                if (CurrentUser != null)
                {
                    var myOrderCountTask = _orderService.CountOrdersByUserAsync(CurrentUser.Id);
                    var myBookCountTask = _bookService.CountBooksByUserAsync(CurrentUser.Id);

                    await Task.WhenAll(myOrderCountTask, myBookCountTask);

                    MyOrderCount = await myOrderCountTask;
                    MyBookCount = await myBookCountTask;
                }
                else
                {
                    MyOrderCount = 0;
                    MyBookCount = 0;
                }

                // Notify property changes
                NotifyAllPropertiesChanged();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard stats: {ex.Message}");
                // Could show a user-friendly error message here
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task RefreshStatsAsync()
        {
            await LoadStatsAsync();
        }

        private void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalBooks));
            OnPropertyChanged(nameof(TotalUsers));
            OnPropertyChanged(nameof(TotalOrders));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalCategories));
            OnPropertyChanged(nameof(BookCount));
            OnPropertyChanged(nameof(OrderCount));
            OnPropertyChanged(nameof(MyOrderCount));
            OnPropertyChanged(nameof(MyBookCount));
        }
        #endregion

        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Cleanup
        public void Cleanup()
        {
            _refreshTimer?.Stop();
        }
        #endregion
    }
} 