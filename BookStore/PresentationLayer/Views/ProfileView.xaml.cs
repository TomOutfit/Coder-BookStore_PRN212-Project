using System.Windows.Controls;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView(ProfileViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
} 