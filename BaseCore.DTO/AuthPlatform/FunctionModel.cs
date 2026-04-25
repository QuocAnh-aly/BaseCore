namespace BaseCore.DTO.AuthPlatform
{
    public class FunctionModel
    {
        public int Id { get; set; }   // ✅ FIX từ string → int

        public string Name { get; set; }

        public string Description { get; set; }

        public bool? IsActive { get; set; }
    }
}