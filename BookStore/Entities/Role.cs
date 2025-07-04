using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <summary>
    /// Represents a role for users in the system.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

        // Constructors
        public Role() { }

        public Role(Role other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            CreatedAt = other.CreatedAt;
            UpdatedAt = other.UpdatedAt;
        }
    }
} 