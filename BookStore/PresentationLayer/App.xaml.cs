using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using BusinessLayer.Services;
using System.Windows;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace PresentationLayer
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public User? CurrentUser { get; set; }

        public App()
        {
            var services = new ServiceCollection();
            string connStr = ConfigurationManager.ConnectionStrings["BookStoreDB"].ConnectionString;
            
            // Đăng ký DbContextFactory để các repository dùng factory tạo context mới cho mỗi thao tác
            services.AddDbContextFactory<BookStoreDBContext>(options =>
                options.UseSqlServer(connStr)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors());
            
            // Đăng ký repository (dùng factory)
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Đăng ký service
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            
            // Đăng ký ViewModel
            services.AddScoped<ViewModels.BookManagementViewModel>();
            services.AddScoped<ViewModels.UserManagementViewModel>();
            services.AddScoped<ViewModels.OrderManagementViewModel>();
            services.AddScoped<ViewModels.CategoryManagementViewModel>();
            services.AddScoped<ViewModels.ProfileViewModel>();
            services.AddScoped<ViewModels.DashboardViewModel>();
            services.AddScoped<ViewModels.WelcomeViewModel>();
            services.AddScoped<ViewModels.MyOrdersViewModel>();
            
            ServiceProvider = services.BuildServiceProvider();
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Hiển thị form đăng nhập cho đến khi đăng nhập thành công hoặc người dùng chọn thoát
            var login = new Views.LoginView();
            var result = login.ShowDialog();
            
            if (result == true)
            {
                // Đăng nhập thành công, LoginView đã đóng hoàn toàn
                var sp = ServiceProvider;
                System.Diagnostics.Debug.WriteLine("Getting ViewModels from ServiceProvider...");
                
                var bookVm = sp.GetRequiredService<ViewModels.BookManagementViewModel>();
                var userVm = sp.GetRequiredService<ViewModels.UserManagementViewModel>();
                var orderVm = sp.GetRequiredService<ViewModels.OrderManagementViewModel>();
                var categoryVm = sp.GetRequiredService<ViewModels.CategoryManagementViewModel>();
                var profileVm = sp.GetRequiredService<ViewModels.ProfileViewModel>();
                var dashboardVm = sp.GetRequiredService<ViewModels.DashboardViewModel>();
                var myOrdersVm = sp.GetRequiredService<ViewModels.MyOrdersViewModel>();
                
                System.Diagnostics.Debug.WriteLine("All ViewModels created successfully");
                
                var mainWindow = new MainWindow(bookVm, userVm, orderVm, categoryVm, profileVm, dashboardVm, myOrdersVm);
                System.Diagnostics.Debug.WriteLine("MainWindow created, showing...");
                
                // Set as main window to prevent immediate closing
                this.MainWindow = mainWindow;
                
                try
                {
                    mainWindow.Show();
                    System.Diagnostics.Debug.WriteLine("MainWindow shown successfully");
                    mainWindow.Closed += (s, e2) => 
                    {
                        System.Diagnostics.Debug.WriteLine("MainWindow was closed");
                        Shutdown();
                    };
                    mainWindow.Loaded += (s, e2) => System.Diagnostics.Debug.WriteLine("MainWindow Loaded event fired in App.xaml.cs");
                    mainWindow.Activated += (s, e2) => System.Diagnostics.Debug.WriteLine("MainWindow Activated event fired");
                    mainWindow.Deactivated += (s, e2) => System.Diagnostics.Debug.WriteLine("MainWindow Deactivated event fired");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error showing MainWindow: {ex.Message}");
                    MessageBox.Show($"Lỗi hiển thị MainWindow: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Login cancelled or failed");
                Shutdown();
                return;
            }
        }
    }
} 