using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class BookManagementView : UserControl
    {
        public BookManagementView(BookManagementViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 