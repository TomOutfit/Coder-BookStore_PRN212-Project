using System.Collections.ObjectModel;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PresentationLayer.Commands;
using System.Threading.Tasks;
using PresentationLayer.Views;
using System.Windows;
using System;
using System.Linq;

namespace PresentationLayer.ViewModels
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public ObservableCollection<OrderDisplayModel> Orders { get; set; } = new();
        private OrderDisplayModel? _selectedOrder;
        public OrderDisplayModel? SelectedOrder { get => _selectedOrder; set { _selectedOrder = value; OnPropertyChanged(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); SearchOrders(); } }
        private int _currentPage = 1;
        public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); LoadOrders(); } }
        public ObservableCollection<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50, 100 };
        private string _goToPageText = "";
        public string GoToPageText { get => _goToPageText; set { _goToPageText = value; OnPropertyChanged(); } }
        public ICommand FirstPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        public bool CanGoNext => CurrentPage < TotalPages;
        public bool CanGoPrevious => CurrentPage > 1;
        private int _pageSize = 10;
        public int PageSize {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadOrders();
                }
            }
        }
        public int TotalPages { get; set; }
        public ICommand AddOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public OrderManagementViewModel(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
            AddOrderCommand = new BookWiseRelayCommand(_ => AddOrder());
            EditOrderCommand = new BookWiseRelayCommand(o => EditOrder(o as OrderDisplayModel), o => o is OrderDisplayModel);
            DeleteOrderCommand = new BookWiseRelayCommand(o => DeleteOrder(o as OrderDisplayModel), o => o is OrderDisplayModel);
            NextPageCommand = new BookWiseRelayCommand(_ => { if (CanGoNext) { CurrentPage++; } }, _ => CanGoNext);
            PrevPageCommand = new BookWiseRelayCommand(_ => { if (CanGoPrevious) { CurrentPage--; } }, _ => CanGoPrevious);
            FirstPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage != 1) { CurrentPage = 1; } }, _ => CanGoPrevious);
            LastPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage != TotalPages) { CurrentPage = TotalPages; } }, _ => CanGoNext);
            GoToPageCommand = new BookWiseRelayCommand(_ => GoToPage(), _ => true);
            RefreshCommand = new BookWiseRelayCommand(_ => RefreshOrders());
            SearchCommand = new BookWiseRelayCommand(_ => SearchOrders());
            _ = LoadOrders();
            DataChangeNotifier.DataChanged += () => LoadOrders();
        }
        private async Task LoadOrders()
        {
            // Lấy toàn bộ đơn hàng (theo trang)
            var result = await _orderService.GetAllOrdersAsync(CurrentPage, PageSize, null); // Không truyền searchText
            var total = await _orderService.CountOrdersAsync(null);
            Orders.Clear();
            var users = (await _userService.GetAllAsync()).ToList();
            foreach (var o in result)
            {
                var user = users.FirstOrDefault(u => u.Id == o.UserId);
                Orders.Add(new OrderDisplayModel
                {
                    Id = o.Id,
                    CustomerName = user != null ? (user.FirstName + " " + user.LastName).Trim() : "Không rõ",
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                });
            }
            // Lọc theo tên khách hàng nếu có search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lower = SearchText.Trim().ToLower();
                var filtered = Orders.Where(o => o.CustomerName.ToLower().Contains(lower)).ToList();
                Orders.Clear();
                foreach (var o in filtered) Orders.Add(o);
                TotalPages = 1;
                CurrentPage = 1;
            }
            else
            {
                TotalPages = (total + PageSize - 1) / PageSize;
            }
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                return;
            }
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoPrevious));
        }
        private async void SearchOrders() { CurrentPage = 1; await LoadOrders(); }
        private async void AddOrder()
        {
            var vm = new OrderDialogViewModel(_orderService, _userService, null, "Thêm đơn hàng");
            var dialog = new OrderDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadOrders();
                DataChangeNotifier.NotifyDataChanged();
                MessageBox.Show("Thêm đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void EditOrder(OrderDisplayModel? order)
        {
            if (order == null) return;
            var orderEntity = await _orderService.GetOrderByIdAsync(order.Id);
            var vm = new OrderDialogViewModel(_orderService, _userService, orderEntity, "Sửa đơn hàng");
            var dialog = new OrderDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadOrders();
                MessageBox.Show("Cập nhật đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void DeleteOrder(OrderDisplayModel? order)
        {
            if (order == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn xóa đơn hàng #{order.Id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    await _orderService.DeleteOrderAsync(order.Id);
                    _ = LoadOrders();
                    DataChangeNotifier.NotifyDataChanged();
                    MessageBox.Show("Xóa đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GoToPage()
        {
            if (int.TryParse(GoToPageText, out int page))
            {
                if (page >= 1 && page <= TotalPages)
                {
                    CurrentPage = page;
                }
                GoToPageText = string.Empty;
            }
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
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class OrderDisplayModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
} 