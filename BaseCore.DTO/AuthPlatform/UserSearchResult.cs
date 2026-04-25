using BaseCore.DTO.Common;
using System.Collections.Generic;

namespace BaseCore.DTO.AuthPlatform
{
    public class UserSearchResult : SearchResult
    {
        public List<UserDto> Records { get; set; }
    }
}