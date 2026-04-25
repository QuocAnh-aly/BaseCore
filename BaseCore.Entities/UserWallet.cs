using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCore.Entities
{
    public class UserWallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSpent { get; set; } = 0;

        public Guid Guid { get; set; } = Guid.NewGuid();
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? Updated { get; set; }

        [Column("UpdatedDateTime")]
        public DateTime UpdatedDateTime { get; set; } = DateTime.Now;
    }
}