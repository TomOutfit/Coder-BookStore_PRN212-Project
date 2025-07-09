using System.Collections.ObjectModel;
using System.Windows.Input;
using BusinessLayer;
using Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PresentationLayer.Commands;
using System.Threading.Tasks;
using BusinessLayer.Services;
using PresentationLayer.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;

namespace PresentationLayer.ViewModels
{
    public class BookManagementViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        public ObservableCollection<Book> Books { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        private Book? _selectedBook;
        public Book? SelectedBook { get => _selectedBook; set { _selectedBook = value; OnPropertyChanged(); } }
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); SearchBooks(); } }
        private int _currentPage = 1;
        public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); LoadBooks(); } }
        public ObservableCollection<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50, 100 };
        private string _goToPageText = "";
        public string GoToPageText { get => _goToPageText; set { _goToPageText = value; OnPropertyChanged(); } }
        public ICommand FirstPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        public bool CanGoNext => CurrentPage < TotalPages;
        public bool CanGoPrevious => CurrentPage > 1;
        private int _pageSize = 10;
        public int PageSize {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = LoadBooks();
                }
            }
        }
        public int TotalPages { get; set; }
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public BookManagementViewModel(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            AddBookCommand = new BookWiseRelayCommand(_ => AddBook());
            EditBookCommand = new BookWiseRelayCommand(b => EditBook(b as Book), b => b is Book);
            DeleteBookCommand = new BookWiseRelayCommand(b => DeleteBook(b as Book), b => b is Book);
            NextPageCommand = new BookWiseRelayCommand(_ => { if (CanGoNext) { CurrentPage++; } }, _ => CanGoNext);
            PrevPageCommand = new BookWiseRelayCommand(_ => { if (CanGoPrevious) { CurrentPage--; } }, _ => CanGoPrevious);
            FirstPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage != 1) { CurrentPage = 1; } }, _ => CanGoPrevious);
            LastPageCommand = new BookWiseRelayCommand(_ => { if (CurrentPage != TotalPages) { CurrentPage = TotalPages; } }, _ => CanGoNext);
            GoToPageCommand = new BookWiseRelayCommand(_ => GoToPage(), _ => true);
            RefreshCommand = new BookWiseRelayCommand(_ => RefreshBooks());
            SearchCommand = new BookWiseRelayCommand(_ => SearchBooks());
            _ = InitAsync();
            DataChangeNotifier.DataChanged += () => LoadBooks();
        }
        private async Task InitAsync()
        {
            await LoadCategories();
            await LoadBooks();
        }
        private async Task LoadBooks()
        {
            var result = await _bookService.GetAllBooksAsync(CurrentPage, PageSize, SearchText);
            var total = await _bookService.CountBooksAsync(SearchText);
            Books.Clear();
            foreach (var b in result) Books.Add(b);
            TotalPages = (total + PageSize - 1) / PageSize;
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                return;
            }
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoPrevious));
        }
        private async Task LoadCategories()
        {
            Categories.Clear();
            var cats = await _categoryService.GetAllCategoriesAsync();
            foreach (var c in cats) Categories.Add(c);
        }
        private async void SearchBooks() { CurrentPage = 1; await LoadBooks(); }
        private async void AddBook()
        {
            var vm = new BookDialogViewModel(_bookService, _categoryService, null, "Thêm sách");
            var dialog = new BookDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadBooks();
                DataChangeNotifier.NotifyDataChanged();
                MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void EditBook(Book? book)
        {
            if (book == null) return;
            var vm = new BookDialogViewModel(_bookService, _categoryService, book, "Sửa sách");
            var dialog = new BookDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadBooks();
                DataChangeNotifier.NotifyDataChanged();
                MessageBox.Show("Cập nhật sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void DeleteBook(Book? book)
        {
            if (book == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn xóa sách '{book.Title}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    await _bookService.DeleteBookAsync(book.Id);
                    _ = LoadBooks();
                    DataChangeNotifier.NotifyDataChanged();
                    MessageBox.Show("Xóa sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GoToPage()
        {
            if (int.TryParse(GoToPageText, out int page))
            {
                if (page >= 1 && page <= TotalPages)
                {
                    CurrentPage = page;
                }
                GoToPageText = string.Empty;
            }
        }
        private async void RefreshBooks()
        {
            try
            {
                await LoadBooks();
                MessageBox.Show("Làm mới danh sách sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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