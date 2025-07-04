using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Represents an order placed by a user or guest.
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(255)]
        public string? ShippingAddress { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();

        // Constructors
        public Order() { }

        public Order(Order other)
        {
            Id = other.Id;
            UserId = other.UserId;
            OrderDate = other.OrderDate;
            TotalAmount = other.TotalAmount;
            Status = other.Status;
            ShippingAddress = other.ShippingAddress;
            PaymentMethod = other.PaymentMethod;
            Notes = other.Notes;
            CreatedAt = other.CreatedAt;
            UpdatedAt = other.UpdatedAt;
        }
    }
} 