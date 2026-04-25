namespace BaseCore.DTO.AuthPlatform
{
    public class RoleModuleFunctionDto
    {
        public int Id { get; set; }

        public int ModuleFunctionId { get; set; }

        public int FunctionId { get; set; }

        public int RoleId { get; set; }

        public int ModuleId { get; set; }

        public bool IsActive { get; set; }
    }
}