using System.Windows.Controls;
using System.Windows;

namespace PresentationLayer.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(ViewModels.DashboardViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += DashboardView_Loaded;
            vm.RequestNavigate += tag =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.GetType().GetMethod("LoadView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(mainWindow, new object[] { tag });
                }
            };
        }

        private void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as PresentationLayer.App;
            bool isAdmin = string.Equals(app?.CurrentUser?.Role, "Admin", System.StringComparison.OrdinalIgnoreCase);
            AdminStatsPanel.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            NoPermissionPanel.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
        }
    }
} 