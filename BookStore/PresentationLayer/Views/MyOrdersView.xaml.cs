using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class MyOrdersView : UserControl
    {
        public MyOrdersView(MyOrdersViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestNavigate += tag =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.GetType().GetMethod("LoadView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(mainWindow, new object[] { tag });
                }
            };
        }
    }
} 