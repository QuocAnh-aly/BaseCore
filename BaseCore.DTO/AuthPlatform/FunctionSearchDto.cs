using BaseCore.DTO.Common;

namespace BaseCore.DTO.AuthPlatform
{
    public class FunctionSearchDto : Paging
    {
        public SortColumn SortColumn { get; set; }
        public FunctionModel Data { get; set; }
    }
}