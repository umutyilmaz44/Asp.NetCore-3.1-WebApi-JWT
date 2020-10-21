using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class UserTypeBo
    {
        public UserTypeBo()
        {
            User = new HashSet<UserBo>();
        }

        public long Id { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public int TokenLifeTime { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserBo> User { get; set; }
    }
}
