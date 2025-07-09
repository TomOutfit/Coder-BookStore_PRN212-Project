using System.Windows;

namespace PresentationLayer.Views
{
    public partial class OrderDialog : Window
    {
        public OrderDialog(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 