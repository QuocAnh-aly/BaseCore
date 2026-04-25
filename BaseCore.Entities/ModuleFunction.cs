using BaseCore.Common;
using BaseCore.Entities.Audit;
using System;
using System.Collections.Generic;

namespace BaseCore.Entities
{
    public partial class ModuleFunction : BaseEntity, IAuditable
    {
        public ModuleFunction()
        {
            RoleModuleFunction = new HashSet<RoleModuleFunction>();
        }

        public Guid Guid { get; set; }

        public int ModuleId { get; set; }

        public int FunctionId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; }

        public DateTime Modified { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Function Function { get; set; }

        public virtual Module Module { get; set; }

        public virtual ICollection<RoleModuleFunction> RoleModuleFunction { get; set; }
    }
}