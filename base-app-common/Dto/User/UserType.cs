using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.user
{
    public class UserTypeDto
    {
        public UserTypeDto()
        {
            User = new HashSet<UserDto>();
        }

        public long Id { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public int TokenLifeTime { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserDto> User { get; set; }
    }
}
