using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BusinessLayer.Services;
using Entities;
using PresentationLayer.Commands;

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
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ObservableCollection<string> GenreOptions { get; set; } = new();
        public ObservableCollection<string> AuthorOptions { get; set; } = new();
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
            return !string.IsNullOrWhiteSpace(Book.Title) && !string.IsNullOrWhiteSpace(Book.Genre) && Book.Price > 0 && Book.Stock >= 0;
        }
        private async void Save()
        {
            ErrorMessage = string.Empty;
            try
            {
                Book.Genre = SelectedCategory?.Name;
                if (Book.Id == 0)
                    await _bookService.AddBookAsync(Book);
                else
                    await _bookService.UpdateBookAsync(Book);
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