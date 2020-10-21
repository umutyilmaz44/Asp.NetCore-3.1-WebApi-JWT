using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.organization
{
    public class OrganizationDto
    {
        public OrganizationDto()
        {            
        }

        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RecordDate { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserDto> User { get; set; }
    }
}
