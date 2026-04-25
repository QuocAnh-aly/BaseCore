using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseCore.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public byte[]? Salt { get; set; }

        public string? Contact { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Position { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserType { get; set; } = 0;

        public DateTime Created { get; set; } = DateTime.Now;

        [NotMapped]
        public decimal Balance { get; set; } = 0;
    }
}