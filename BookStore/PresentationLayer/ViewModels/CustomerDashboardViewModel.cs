using BusinessLayer.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PresentationLayer.ViewModels;

namespace PresentationLayer.ViewModels
{
    public class CustomerDashboardViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IBookService _bookService;

        public CustomerDashboardViewModel(IOrderService orderService, IBookService bookService)
        {
            _orderService = orderService;
            _bookService = bookService;

            MyOrderCount = 0;
            MyBookCount = 0;

            GoToMyOrdersCommand = new RelayCommand<object?>(GoToMyOrders);
            GoToProfileCommand = new RelayCommand<object?>(GoToProfile);
        }

        public int MyOrderCount { get; set; }
        public int MyBookCount { get; set; }

        public ICommand GoToMyOrdersCommand { get; }
        public ICommand GoToProfileCommand { get; }

        private void GoToMyOrders(object? parameter)
        {
            // Implementation of GoToMyOrders method
        }

        private void GoToProfile(object? parameter)
        {
            // Implementation of GoToProfile method
        }
    }
} 