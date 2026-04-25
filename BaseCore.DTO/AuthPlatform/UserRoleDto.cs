namespace BaseCore.DTO.AuthPlatform
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }
    }
}