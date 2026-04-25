using BaseCore.DTO.Common;

namespace BaseCore.DTO.AuthPlatform
{
    public class UserSearchDto : Paging
    {
        public SortColumn SortColumn { get; set; }

        public InsertUserParam Data { get; set; }
    }
}