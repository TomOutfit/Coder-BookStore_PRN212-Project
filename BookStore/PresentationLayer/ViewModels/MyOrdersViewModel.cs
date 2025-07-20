using System.ComponentModel;
using System.Runtime.CompilerServices;
using BusinessLayer.Services;
using System.Windows;
using System.Windows.Input;
using PresentationLayer.Commands;
using Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class MyOrdersViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        public event PropertyChangedEventHandler? PropertyChanged;
        private User? CurrentUser => (System.Windows.Application.Current as PresentationLayer.App)?.CurrentUser;

        public ObservableCollection<Order> MyOrders { get; set; }
        public Order? SelectedOrder { get; set; }
        public ObservableCollection<OrderDetail> OrderDetails { get; set; }

        public ICommand RefreshCommand { get; set; }
        public ICommand ViewOrderDetailsCommand { get; set; }
        public ICommand BackToDashboardCommand { get; set; }
        public event Action<string>? RequestNavigate;

        public MyOrdersViewModel(IOrderService orderService, IOrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            MyOrders = new ObservableCollection<Order>();
            OrderDetails = new ObservableCollection<OrderDetail>();

            RefreshCommand = new BookWiseRelayCommand(_ => LoadMyOrders());
            ViewOrderDetailsCommand = new BookWiseRelayCommand(order => ViewOrderDetails(order as Order));
            BackToDashboardCommand = new BookWiseRelayCommand(_ => BackToDashboard());

            LoadMyOrders();
        }

        private async void LoadMyOrders()
        {
            if (CurrentUser == null) return;

            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(CurrentUser.Id);
                MyOrders.Clear();
                foreach (var order in orders)
                {
                    MyOrders.Add(order);
                }
                OnPropertyChanged(nameof(MyOrders));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewOrderDetails(Order? order)
        {
            if (order == null) return;

            SelectedOrder = order;
            OnPropertyChanged(nameof(SelectedOrder));

            try
            {
                var details = await _orderDetailService.GetOrderDetailsByOrderIdAsync(SelectedOrder.Id);
                OrderDetails.Clear();
                foreach (var detail in details)
                {
                    OrderDetails.Add(detail);
                }
                OnPropertyChanged(nameof(OrderDetails));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToDashboard()
        {
            RequestNavigate?.Invoke("Dashboard");
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 