using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseCore.DTO.AuthPlatform
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime Modified { get; set; }

        public int UserCount { get; set; }

        public int RoleType { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsMember { get; set; }

        public bool? IsActive { get; set; }

        public ICollection<ModuleDto> RoleModule { get; set; }
    }
}