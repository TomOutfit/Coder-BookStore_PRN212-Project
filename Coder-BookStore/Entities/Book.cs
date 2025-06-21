using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    [Index(nameof(ISBN), IsUnique = true)]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Author { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string ISBN { get; set; }

        [StringLength(50)]
        public string Genre { get; set; }

        [StringLength(int.MaxValue)]
        public string Description { get; set; }

        [Required]
        public int Stock { get; set; }

        [StringLength(100)]
        public string Publisher { get; set; }

        [StringLength(50)]
        public string Language { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }
}