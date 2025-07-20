using System.Windows;
using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class DashboardView : UserControl
    {
        private int _previousTabIndex = 0;

        public DashboardView(DashboardViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += DashboardView_Loaded;
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
            vm.RequestNavigate += tag =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.GetType().GetMethod("LoadView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(mainWindow, new object[] { tag });
            };
        }

        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            // Tự động chọn tab phù hợp khi load
            if (DataContext is DashboardViewModel vm)
            {
                if (vm.IsCustomer) 
                {
                    MainTabControl.SelectedIndex = 2;
                    _previousTabIndex = 2;
                }
                else if (vm.IsStaff) 
                {
                    MainTabControl.SelectedIndex = 1;
                    _previousTabIndex = 1;
                }
                else 
                {
                    MainTabControl.SelectedIndex = 0;
                    _previousTabIndex = 0;
                }
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is DashboardViewModel vm)) return;
            var tab = MainTabControl.SelectedItem as TabItem;
            if (tab == null) return;
            string header = tab.Header?.ToString() ?? "";

            // Admin: vào mọi tab
            if (vm.IsAdmin) 
            {
                _previousTabIndex = MainTabControl.SelectedIndex;
                return;
            }

            // Staff: không vào Admin
            if (vm.IsStaff && header == "Admin")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Bạn không có quyền truy cập tab Admin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MainTabControl.SelectedIndex = _previousTabIndex; // Quay về tab trước đó
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                return;
            }

            // Customer: chỉ vào Customer
            if (vm.IsCustomer && (header == "Admin" || header == "Staff"))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Bạn không có quyền truy cập tab này!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MainTabControl.SelectedIndex = _previousTabIndex; // Quay về tab trước đó
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                return;
            }

            // Nếu có quyền truy cập, cập nhật tab trước đó
            _previousTabIndex = MainTabControl.SelectedIndex;
        }
    }
} 