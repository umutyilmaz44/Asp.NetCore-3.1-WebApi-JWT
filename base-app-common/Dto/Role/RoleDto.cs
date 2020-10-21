using base_app_common.dto.grand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.role
{
    public class RoleDto
    {
        public RoleDto()
        {
            Grand = new HashSet<GrandDto>();
        }

        public long Id { get; set; }
        public string RoleName { get; set; }
        public string Desc { get; set; }
        [JsonIgnore]
        public virtual ICollection<GrandDto> Grand { get; set; }
    }
}
