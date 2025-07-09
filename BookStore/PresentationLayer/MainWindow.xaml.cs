using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer
{
    public partial class MainWindow : Window
    {
        private readonly ViewModels.BookManagementViewModel _bookVm;
        private readonly ViewModels.UserManagementViewModel _userVm;
        private readonly ViewModels.OrderManagementViewModel _orderVm;
        private readonly ViewModels.CategoryManagementViewModel _categoryVm;
        private readonly ViewModels.ProfileViewModel _profileVm;
        private readonly ViewModels.DashboardViewModel _dashboardVm;
        private readonly ViewModels.WelcomeViewModel _welcomeVm;
        public MainWindow(
            ViewModels.BookManagementViewModel bookVm,
            ViewModels.UserManagementViewModel userVm,
            ViewModels.OrderManagementViewModel orderVm,
            ViewModels.CategoryManagementViewModel categoryVm,
            ViewModels.ProfileViewModel profileVm,
            ViewModels.DashboardViewModel dashboardVm)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("MainWindow constructor started");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("MainWindow InitializeComponent completed");
                
                _bookVm = bookVm;
                _userVm = userVm;
                _orderVm = orderVm;
                _categoryVm = categoryVm;
                _profileVm = profileVm;
                _dashboardVm = dashboardVm;
                // Lấy WelcomeViewModel từ DI container
                var app = System.Windows.Application.Current as PresentationLayer.App;
                _welcomeVm = app?.ServiceProvider.GetService(typeof(ViewModels.WelcomeViewModel)) as ViewModels.WelcomeViewModel;
                
                System.Diagnostics.Debug.WriteLine("ViewModels assigned, loading Welcome view");
                LoadView("Welcome");
                System.Diagnostics.Debug.WriteLine("MainWindow constructor completed");
                
                // Thêm event handlers để theo dõi
                this.Loaded += (s, e) => System.Diagnostics.Debug.WriteLine("MainWindow Loaded event fired");
                this.Closing += (s, e) => 
                {
                    System.Diagnostics.Debug.WriteLine("MainWindow Closing event fired");
                    System.Diagnostics.Debug.WriteLine($"Closing event args: {e.GetType().Name}");
                };
                this.Closed += (s, e) => System.Diagnostics.Debug.WriteLine("MainWindow Closed event fired");
                this.Activated += (s, e) => System.Diagnostics.Debug.WriteLine("MainWindow Activated event fired");
                this.Deactivated += (s, e) => System.Diagnostics.Debug.WriteLine("MainWindow Deactivated event fired");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in MainWindow constructor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                if (tag == "Logout")
                {
                    // Quay lại LoginView
                    var login = new Views.LoginView();
                    login.Show();
                    this.Close();
                    return;
                }
                LoadView(tag);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.LoginView();
            login.Show();
            this.Close();
        }

        private void LoadView(string tag)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadView called with tag: {tag}");
                var app = System.Windows.Application.Current as PresentationLayer.App;
                if (tag == "Profile" && app?.CurrentUser != null)
                {
                    _profileVm.LoadUser(app.CurrentUser);
                }
                UserControl view = tag switch
                {
                    "Welcome" => new Views.WelcomeView(_welcomeVm),
                    "Dashboard" => new Views.DashboardView(_dashboardVm),
                    "Books" => new Views.BookManagementView(_bookVm),
                    "Users" => new Views.UserManagementView(_userVm),
                    "Orders" => new Views.OrderManagementView(_orderVm),
                    "Categories" => new Views.CategoryManagementView(_categoryVm),
                    "Profile" => new Views.ProfileView(_profileVm),
                    _ => new Views.DashboardView(_dashboardVm),
                };
                System.Diagnostics.Debug.WriteLine($"View created: {view.GetType().Name}");
                MainContent.Content = view;
                System.Diagnostics.Debug.WriteLine("View assigned to MainContent");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in LoadView: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 