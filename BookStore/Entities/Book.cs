using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Entities
{
    /// <summary>
    /// Represents a book entity in the bookstore system.
    /// </summary>
    [Index(nameof(ISBN), IsUnique = true)]
    public class Book : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [Key]
        public int Id { get; set; }

        private string _title;
        [Required]
        [StringLength(255)]
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string? _author;
        [StringLength(255)]
        public string? Author
        {
            get => _author;
            set { _author = value; OnPropertyChanged(); }
        }

        private decimal _price;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        private DateTime? _publishedDate;
        [DataType(DataType.Date)]
        public DateTime? PublishedDate
        {
            get => _publishedDate;
            set { _publishedDate = value; OnPropertyChanged(); }
        }

        private string _isbn;
        [Required]
        [StringLength(20)]
        public string ISBN
        {
            get => _isbn;
            set { _isbn = value; OnPropertyChanged(); }
        }

        private string? _genre;
        [StringLength(50)]
        public string? Genre
        {
            get => _genre;
            set { _genre = value; OnPropertyChanged(); }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private int _stock;
        [Required]
        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(); }
        }

        private string? _publisher;
        [StringLength(100)]
        public string? Publisher
        {
            get => _publisher;
            set { _publisher = value; OnPropertyChanged(); }
        }

        private string? _language;
        [StringLength(50)]
        public string? Language
        {
            get => _language;
            set { _language = value; OnPropertyChanged(); }
        }

        private DateTime _createdAt;
        [Required]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        private DateTime _updatedAt;
        [Required]
        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(); }
        }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        // Constructors
        public Book()
        {
            _title = string.Empty;
            _isbn = string.Empty;
            _createdAt = DateTime.UtcNow;
            _updatedAt = DateTime.UtcNow;
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Book(Book other) : this()
        {
            Id = other.Id;
            Title = other.Title;
            Author = other.Author;
            Price = other.Price;
            PublishedDate = other.PublishedDate;
            ISBN = other.ISBN;
            Genre = other.Genre;
            Description = other.Description;
            Stock = other.Stock;
            Publisher = other.Publisher;
            Language = other.Language;
            CreatedAt = other.CreatedAt;
            UpdatedAt = other.UpdatedAt;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 