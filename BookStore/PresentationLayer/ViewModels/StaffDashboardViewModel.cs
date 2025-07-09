using System.Windows.Input;
using BusinessLayer.Services;
using Microsoft.Extensions.DependencyInjection;
using PresentationLayer.ViewModels;
using PresentationLayer.Commands;
using System.ComponentModel;

namespace PresentationLayer.ViewModels
{
    public class StaffDashboardViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;

        public StaffDashboardViewModel(IServiceProvider serviceProvider)
        {
            _bookService = serviceProvider.GetRequiredService<IBookService>();
            _orderService = serviceProvider.GetRequiredService<IOrderService>();
        }

        public int BookCount { get; set; }
        public int OrderCount { get; set; }

        public ICommand GoToBooksCommand => new AsyncCommand(async _ => await GoToBooksAsync());
        public ICommand GoToOrdersCommand => new AsyncCommand(async _ => await GoToOrdersAsync());

        private async Task GoToBooksAsync()
        {
            // Implementation of GoToBooksAsync method
            await Task.CompletedTask;
        }

        private async Task GoToOrdersAsync()
        {
            // Implementation of GoToOrdersAsync method
            await Task.CompletedTask;
        }
    }
} 