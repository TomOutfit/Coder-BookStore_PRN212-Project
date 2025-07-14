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
    public class OrderDialogViewModel : INotifyPropertyChanged
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public Order Order { get; set; }
        public ObservableCollection<OrderDetail> OrderDetails { get; set; }
        public ObservableCollection<User> Users { get; set; } = new();
        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                    // Đồng bộ UserId cho Order
                    if (_selectedUser != null)
                        Order.UserId = _selectedUser.Id;
                }
            }
        }
        public ObservableCollection<string> StatusList { get; set; }
        public string DialogTitle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public OrderDialogViewModel(IOrderService orderService, IUserService userService, Order? order = null, string title = "Thêm đơn hàng")
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            DialogTitle = title;
            Order = order != null ? new Order
            {
                Id = order.Id,
                User = order.User,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetail
                {
                    Id = od.Id,
                    BookId = od.BookId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            } : new Order { OrderDate = System.DateTime.Now };
            OrderDetails = new ObservableCollection<OrderDetail>(Order.OrderDetails ?? new System.Collections.Generic.List<OrderDetail>());
            StatusList = new ObservableCollection<string>(new[] { "Pending", "Completed", "Cancelled" });
            SaveCommand = new BookWiseRelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
            LoadUsers(); // Luôn load khách hàng khi mở dialog
        }
        private async void LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync(1, 100);
            Users = new ObservableCollection<User>(users);
            // Đồng bộ SelectedUser với Order.UserId
            if (Order != null && Order.UserId > 0)
                SelectedUser = Users.FirstOrDefault(u => u.Id == Order.UserId);
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(SelectedUser));
        }
        private bool CanSave()
        {
            return SelectedUser != null && OrderDetails.Count > 0 && !string.IsNullOrWhiteSpace(Order.Status);
        }
        private async void Save()
        {
            ErrorMessage = string.Empty;
            try
            {
                if (SelectedUser != null)
                {
                    Order.UserId = SelectedUser.Id;
                    Order.OrderDetails = OrderDetails.ToList();
                    await _orderService.UpdateOrderAsync(Order);
                    CloseDialog(true);
                }
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
                if (w is PresentationLayer.Views.OrderDialog dialog && dialog.DataContext == this)
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