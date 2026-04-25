using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCore.Entities
{
    public class GameAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountPassword { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Available";

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(100)]
        public string CreatedUser { get; set; }

        // Additional fields for marketplace functionality
        [MaxLength(100)]
        public string? GameName { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsSold { get; set; } = false;

        public int? SellerId { get; set; }

        [ForeignKey("SellerId")]
        public User Seller { get; set; }

        public int? BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public User Buyer { get; set; }

        public DateTime? SoldAt { get; set; }
    }
}