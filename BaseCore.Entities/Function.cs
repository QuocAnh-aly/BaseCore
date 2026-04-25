using BaseCore.Common;
using System.Collections.Generic;

namespace BaseCore.Entities
{
    public partial class Function : BaseEntity
    {
        public Function()
        {
            ModuleFunction = new HashSet<ModuleFunction>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public bool IsChecked { get; set; }

        public virtual ICollection<ModuleFunction> ModuleFunction { get; set; }
    }
}