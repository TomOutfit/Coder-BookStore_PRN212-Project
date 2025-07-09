using System.Collections.ObjectModel;
using System.Windows.Input;
using Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PresentationLayer.Commands;
using BusinessLayer.Services;
using PresentationLayer.Views;
using System.Windows;
using System;

namespace PresentationLayer.ViewModels
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public ObservableCollection<Order> Orders { get; set; } = new();
        public ObservableCollection<OrderDisplayModel> OrdersDisplay { get; set; } = new();
        private Order? _selectedOrder;
        public Order? SelectedOrder { get => _selectedOrder; set { _selectedOrder = value; OnPropertyChanged(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); SearchOrders(); } }
        private int _currentPage = 1;
        public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); LoadOrders(); } }
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public ObservableCollection<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50, 100 };
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand AddOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public OrderManagementViewModel(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
            AddCommand = new BookWiseRelayCommand(_ => AddOrder());
            EditCommand = new BookWiseRelayCommand(_ => EditOrder(), _ => SelectedOrder != null);
            DeleteCommand = new BookWiseRelayCommand(_ => DeleteOrder(), _ => SelectedOrder != null);
            NextPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage < TotalPages) { CurrentPage++; } });
            PrevPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage > 1) { CurrentPage--; } });
            AddOrderCommand = new BookWiseRelayCommand(_ => AddOrder());
            EditOrderCommand = new BookWiseRelayCommand(o => EditOrder(o as Order), o => o is Order);
            DeleteOrderCommand = new BookWiseRelayCommand(o => DeleteOrder(o as Order), o => o is Order);
            SearchCommand = new BookWiseRelayCommand(_ => SearchOrders());
            RefreshCommand = new BookWiseRelayCommand(_ => RefreshOrders());
            _ = LoadOrders();
            DataChangeNotifier.DataChanged += () => LoadOrders();
        }
        private async Task LoadOrders()
        {
            var result = await _orderService.GetAllOrdersAsync(CurrentPage, PageSize, SearchText);
            var total = await _orderService.CountOrdersAsync(SearchText);
            Orders.Clear();
            OrdersDisplay.Clear();
            var users = (await _userService.GetAllAsync()).ToList();
            foreach (var o in result)
            {
                Orders.Add(o);
                var user = users.FirstOrDefault(u => u.Id == o.UserId);
                OrdersDisplay.Add(new OrderDisplayModel
                {
                    OrderId = o.Id,
                    CustomerName = user != null ? (user.FirstName + " " + user.LastName).Trim() : "Không rõ",
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                    // Thêm các property khác nếu cần
                });
            }
            TotalPages = (total + PageSize - 1) / PageSize;
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(OrdersDisplay));
        }
        private async void SearchOrders() { CurrentPage = 1; await LoadOrders(); }
        private void AddOrder()
        {
            var vm = new OrderDialogViewModel(_orderService, _userService, null, "Thêm đơn hàng");
            var dialog = new OrderDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                LoadOrders();
                PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                MessageBox.Show("Thêm đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DataChangeNotifier.NotifyDataChanged();
            }
        }
        private void EditOrder(Order? order)
        {
            if (order == null) return;
            var vm = new OrderDialogViewModel(_orderService, _userService, order, "Sửa đơn hàng");
            var dialog = new OrderDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                LoadOrders();
                PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                MessageBox.Show("Cập nhật đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DataChangeNotifier.NotifyDataChanged();
            }
        }
        private async void DeleteOrder(Order? order)
        {
            if (order == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn xóa đơn hàng #{order.Id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    await _orderService.DeleteOrderAsync(order.Id);
                    LoadOrders();
                    PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                    MessageBox.Show("Xóa đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataChangeNotifier.NotifyDataChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void EditOrder()
        {
            if (SelectedOrder != null)
                EditOrder(SelectedOrder);
        }
        private void DeleteOrder()
        {
            if (SelectedOrder != null)
                DeleteOrder(SelectedOrder);
        }
        private async void RefreshOrders()
        {
            try
            {
                await LoadOrders();
                MessageBox.Show("Làm mới danh sách đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class OrderDisplayModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        // Thêm các property khác nếu cần
    }
} 