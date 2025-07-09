using System.Windows;

namespace PresentationLayer.Views
{
    public partial class CategoryDialog : Window
    {
        public CategoryDialog(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 