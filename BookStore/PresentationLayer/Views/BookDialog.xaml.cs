using System.Windows;

namespace PresentationLayer.Views
{
    public partial class BookDialog : Window
    {
        public BookDialog(object vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 