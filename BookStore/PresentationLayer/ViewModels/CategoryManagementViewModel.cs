using System.Collections.ObjectModel;
using System.Windows.Input;
using BusinessLayer;
using Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PresentationLayer.Commands;
using System.Threading.Tasks;
using System.Linq;
using BusinessLayer.Services;
using PresentationLayer.Views;
using System.Windows;
using System;

namespace PresentationLayer.ViewModels
{
    public class CategoryManagementViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;
        public ObservableCollection<Category> Categories { get; set; } = new();
        private Category? _selectedCategory;
        public Category? SelectedCategory { get => _selectedCategory; set { _selectedCategory = value; OnPropertyChanged(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); SearchCategories(); } }
        private int _currentPage = 1;
        public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); LoadCategories(); } }
        private int _pageSize = 10;
        public int PageSize {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadCategories();
                }
            }
        }
        public int TotalPages { get; set; }
        public ObservableCollection<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50, 100 };
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public CategoryManagementViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
            AddCommand = new BookWiseRelayCommand(_ => AddCategory());
            EditCommand = new BookWiseRelayCommand(_ => EditCategory(), _ => SelectedCategory != null);
            DeleteCommand = new BookWiseRelayCommand(_ => DeleteCategory(), _ => SelectedCategory != null);
            NextPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage < TotalPages) { CurrentPage++; } });
            PrevPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage > 1) { CurrentPage--; } });
            AddCategoryCommand = new BookWiseRelayCommand(_ => AddCategory());
            EditCategoryCommand = new BookWiseRelayCommand(c => EditCategory(c as Category), c => c is Category);
            DeleteCategoryCommand = new BookWiseRelayCommand(c => DeleteCategory(c as Category), c => c is Category);
            SearchCommand = new BookWiseRelayCommand(_ => SearchCategories());
            RefreshCommand = new BookWiseRelayCommand(_ => RefreshCategories());
            _ = LoadCategories();
            DataChangeNotifier.DataChanged += () => LoadCategories();
        }
        private async Task LoadCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync(SearchText);
            var total = await _categoryService.CountCategoriesAsync(SearchText);
            Categories.Clear();
            int skip = (CurrentPage - 1) * PageSize;
            foreach (var c in result.Skip(skip).Take(PageSize)) Categories.Add(c);
            TotalPages = (total + PageSize - 1) / PageSize;
            OnPropertyChanged(nameof(TotalPages));
        }
        private async void SearchCategories() { CurrentPage = 1; await LoadCategories(); }
        private async void AddCategory()
        {
            var vm = new CategoryDialogViewModel(_categoryService, null, "Thêm thể loại");
            var dialog = new CategoryDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                LoadCategories();
                PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                MessageBox.Show("Thêm thể loại thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DataChangeNotifier.NotifyDataChanged();
            }
        }
        private async void EditCategory(Category? category)
        {
            if (category == null) return;
            var vm = new CategoryDialogViewModel(_categoryService, category, "Sửa thể loại");
            var dialog = new CategoryDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                LoadCategories();
                PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                MessageBox.Show("Cập nhật thể loại thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DataChangeNotifier.NotifyDataChanged();
            }
        }
        private async void DeleteCategory(Category? category)
        {
            if (category == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn xóa thể loại '{category.Name}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    await _categoryService.DeleteCategoryAsync(category.Id);
                    LoadCategories();
                    PresentationLayer.ViewModels.DashboardViewModel.NotifyStatsChanged();
                    MessageBox.Show("Xóa thể loại thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataChangeNotifier.NotifyDataChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void EditCategory()
        {
            if (SelectedCategory != null)
                EditCategory(SelectedCategory);
        }
        private void DeleteCategory()
        {
            if (SelectedCategory != null)
                DeleteCategory(SelectedCategory);
        }
        private async void RefreshCategories()
        {
            try
            {
                await LoadCategories();
                MessageBox.Show("Làm mới danh sách chủ đề thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 