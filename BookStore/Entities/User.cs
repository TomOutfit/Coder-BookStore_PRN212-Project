using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <summary>
    /// Represents a user in the system, either customer or administrator.
    /// </summary>
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Role))]
        public virtual Role? RoleNavigation { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        [NotMapped]
        public string FullName => $"{LastName ?? ""} {FirstName ?? ""}".Trim();

        // Constructors
        public User() { }

        public User(User other)
        {
            Id = other.Id;
            Username = other.Username;
            Password = other.Password;
            Email = other.Email;
            FirstName = other.FirstName;
            LastName = other.LastName;
            Address = other.Address;
            City = other.City;
            State = other.State;
            ZipCode = other.ZipCode;
            PhoneNumber = other.PhoneNumber;
            Role = other.Role;
            IsActive = other.IsActive;
            CreatedAt = other.CreatedAt;
            UpdatedAt = other.UpdatedAt;
        }
    }
} 