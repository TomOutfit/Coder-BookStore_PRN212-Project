using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class CategoryManagementView : UserControl
    {
        public CategoryManagementView(CategoryManagementViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 