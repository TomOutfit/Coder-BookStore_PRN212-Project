using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class UserManagementView : UserControl
    {
        public UserManagementView(UserManagementViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 