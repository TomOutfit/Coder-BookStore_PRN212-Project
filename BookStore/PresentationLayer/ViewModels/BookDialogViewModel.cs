using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using PresentationLayer.Commands;
using System.Windows; // Add this
using System;

namespace PresentationLayer.ViewModels
{
    public class BookDialogViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        public Book Book { get; set; }
        public ObservableCollection<Category> Categories { get; set; } = new();
        private Category? _selectedCategory;
        public Category? SelectedCategory { get => _selectedCategory; set { _selectedCategory = value; Book.Genre = value?.Name; OnPropertyChanged(); } }
        public string DialogTitle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ICommand SaveCommand { get; private set; } // Make private set
        public ICommand CancelCommand { get; }
        public ObservableCollection<string> GenreOptions { get; set; } = new();
        public ObservableCollection<string> AuthorOptions { get; set; } = new();
        public bool IsEdit => Book != null && Book.Id > 0; // Add IsEdit property
        public event PropertyChangedEventHandler? PropertyChanged;
        public BookDialogViewModel(IBookService bookService, ICategoryService categoryService, Book? book = null, string title = "Thêm sách")
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            DialogTitle = title;
            Book = book != null ? new Book
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                ISBN = book.ISBN,
                Genre = book.Genre,
                Description = book.Description,
                Stock = book.Stock,
                Publisher = book.Publisher,
                Language = book.Language
            } : new Book();
            SaveCommand = new BookWiseRelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new BookWiseRelayCommand(_ => Cancel());
            LoadCategories();
            LoadBookOptions();
            if (Book != null)
                Book.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Book));
        }
        private async void LoadCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);
            // Gán Genre nếu có
            if (!string.IsNullOrWhiteSpace(Book.Genre))
                SelectedCategory = Categories.FirstOrDefault(c => c.Name == Book.Genre);
            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(SelectedCategory));
        }
        private async void LoadBookOptions()
        {
            var books = await _bookService.GetAllAsync();
            var genres = books.Select(b => b.Genre).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct().OrderBy(g => g);
            var authors = books.Select(b => b.Author).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct().OrderBy(a => a);
            GenreOptions = new ObservableCollection<string>(genres);
            AuthorOptions = new ObservableCollection<string>(authors);
            OnPropertyChanged(nameof(GenreOptions));
            OnPropertyChanged(nameof(AuthorOptions));
        }
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Book?.Title)
                && !string.IsNullOrWhiteSpace(Book?.Author)
                && !string.IsNullOrWhiteSpace(Book?.Genre)
                && Book?.Price > 0
                && Book?.Stock >= 0; // Use Stock instead of Quantity
        }
        private async void Save()
        {
            try
            {
                if (IsEdit)
                    await _bookService.UpdateBookAsync(Book);
                else
                    await _bookService.AddBookAsync(Book); // Use AddBookAsync
                CloseDialog(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Cancel() => CloseDialog(false);
        private void CloseDialog(bool result)
        {
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is PresentationLayer.Views.BookDialog dialog && dialog.DataContext == this)
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