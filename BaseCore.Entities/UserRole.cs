using BaseCore.Common;
using System;

namespace BaseCore.Entities
{
    public partial class UserRole : BaseEntity
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}