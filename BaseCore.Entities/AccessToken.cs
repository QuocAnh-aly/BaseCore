using BaseCore.Common;
using System;
using System.Collections.Generic;

namespace BaseCore.Entities
{
    public partial class AccessToken: BaseEntity
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        // Foreign key
        public int UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expirated { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<Role> Roles { get; set; }

        public virtual User User { get; set; }
    }
}
