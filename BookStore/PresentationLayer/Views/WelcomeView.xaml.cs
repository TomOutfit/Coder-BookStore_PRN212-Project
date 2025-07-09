using System.Windows.Controls;

namespace PresentationLayer.Views
{
    public partial class WelcomeView : UserControl
    {
        public WelcomeView(ViewModels.WelcomeViewModel vm)
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
} 