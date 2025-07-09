using System;
using System.Windows;
using System.Windows.Input;
using PresentationLayer.Commands;

namespace PresentationLayer.ViewModels
{
    public class WelcomeViewModel
    {
        public ICommand GoToBooksCommand { get; }
        public ICommand GoToUsersCommand { get; }
        public ICommand GoToOrdersCommand { get; }
        public ICommand GoToCategoriesCommand { get; }
        public ICommand StartCommand { get; }
        public event Action<string>? RequestNavigate;

        public WelcomeViewModel()
        {
            GoToBooksCommand = new BookWiseRelayCommand(_ => GoToBooks());
            GoToUsersCommand = new BookWiseRelayCommand(_ => GoToUsers());
            GoToOrdersCommand = new BookWiseRelayCommand(_ => GoToOrders());
            GoToCategoriesCommand = new BookWiseRelayCommand(_ => GoToCategories());
            StartCommand = new BookWiseRelayCommand(_ => GoToDashboard());
        }

        private void GoToBooks()
        {
            RequestNavigate?.Invoke("Books");
        }
        private void GoToUsers()
        {
            RequestNavigate?.Invoke("Users");
        }
        private void GoToOrders()
        {
            RequestNavigate?.Invoke("Orders");
        }
        private void GoToCategories()
        {
            RequestNavigate?.Invoke("Categories");
        }
        private void GoToDashboard()
        {
            RequestNavigate?.Invoke("Dashboard");
        }
    }
} 