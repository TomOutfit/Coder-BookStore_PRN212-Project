using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using PresentationLayer.Commands;

namespace PresentationLayer.ViewModels
{
    public class CategoryDialogViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;
        public Category Category { get; set; }
        public string DialogTitle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public CategoryDialogViewModel(ICategoryService categoryService, Category? category = null, string title = "Thêm thể loại")
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            DialogTitle = title;
            Category = category != null ? new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            } : new Category();
            SaveCommand = new BookWiseRelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
        }
        
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Category.Name);
        }
        
        private async void Save()
        {
            ErrorMessage = string.Empty;
            try
            {
                if (Category.Id == 0)
                    await _categoryService.AddCategoryAsync(Category);
                else
                    await _categoryService.UpdateCategoryAsync(Category);
                CloseDialog(true);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        
        private void Cancel() => CloseDialog(false);
        
        private void CloseDialog(bool result)
        {
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is PresentationLayer.Views.CategoryDialog dialog && dialog.DataContext == this)
                {
                    dialog.DialogResult = result;
                    dialog.Close();
                    break;
                }
            }
        }
        
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 