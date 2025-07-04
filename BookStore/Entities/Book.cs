using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Represents a book entity in the bookstore system.
    /// </summary>
    [Index(nameof(ISBN), IsUnique = true)]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        [StringLength(255)]
        public string? Author { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string ISBN { get; set; } = null!;

        [StringLength(50)]
        public string? Genre { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Stock { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();

        // Constructors
        public Book() { }

        public Book(Book other)
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
    }
} 