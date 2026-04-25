using BaseCore.Common;

namespace BaseCore.Entities
{
    public partial class RoleModuleFunction : BaseEntity
    {
        public int RoleId { get; set; }

        public int ModuleFunctionId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
    }
}