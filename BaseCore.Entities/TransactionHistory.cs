using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCore.Entities
{
    public enum TransactionType
    {
        Deposit = 1,    // Nạp tiền
        Withdraw = 2,   // Rút tiền
        Purchase = 3,   // Mua tài khoản game
        Sale = 4        // Bán tài khoản game
    }

    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }

    public class TransactionHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [Column("TransactionType")]
        public TransactionType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Column("TransactionStatus")]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public int? GameAccountId { get; set; }

        [ForeignKey("GameAccountId")]
        public GameAccount GameAccount { get; set; }

        [MaxLength(100)]
        public string PaymentMethod { get; set; }

        // [MaxLength(500)]
        // public string TransactionCode { get; set; }

        [Column("TransactionDate")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("CreatedDateTime")]
        public DateTime CompletedAt { get; set; } = DateTime.Now;

        public Guid Guid { get; set; } = Guid.NewGuid();
        public string CreatedBy { get; set; } = "System";
        [Column("Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}