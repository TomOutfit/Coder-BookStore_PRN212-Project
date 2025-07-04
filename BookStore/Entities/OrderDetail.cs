using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Represents the details of an order, linking books to orders.
    /// </summary>
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book? Book { get; set; }

        // Constructors
        public OrderDetail() { }

        public OrderDetail(OrderDetail other)
        {
            Id = other.Id;
            OrderId = other.OrderId;
            BookId = other.BookId;
            Quantity = other.Quantity;
            UnitPrice = other.UnitPrice;
            Subtotal = other.Subtotal;
            CreatedAt = other.CreatedAt;
            UpdatedAt = other.UpdatedAt;
        }
    }
} 