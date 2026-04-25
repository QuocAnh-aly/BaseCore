using System;
using System.Collections.Generic;

namespace BaseCore.DTO.AuthPlatform
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Contact { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string ShortName { get; set; }

        public string Position { get; set; }

        public string Image { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime Modified { get; set; }

        public int UserType { get; set; }

        public string Thumbnail { get; set; }

        public ICollection<RoleDto> RoleUser { get; set; }
    }
}