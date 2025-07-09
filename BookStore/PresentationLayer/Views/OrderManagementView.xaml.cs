using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class OrderManagementView : UserControl
    {
        public OrderManagementView(OrderManagementViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 