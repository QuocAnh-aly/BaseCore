using System.Collections.Generic;

namespace BaseCore.DTO.AuthPlatform
{
    public class ModuleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public bool IsChecked { get; set; }

        public ICollection<FunctionDto> ModuleFunction { get; set; }
    }
}