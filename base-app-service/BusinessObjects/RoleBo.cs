using base_app_common.dto.role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class RoleBo
    {
        public RoleBo()
        {
            UserRole = new HashSet<UserRoleBo>();
            GrandRole = new HashSet<GrandRoleBo>();
        }

        public long Id { get; set; }
        public string RoleName { get; set; }
        public string Desc { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserRoleBo> UserRole { get; set; }
        [JsonIgnore]
        public virtual ICollection<GrandRoleBo> GrandRole { get; set; }

        public static RoleDto ConvertToDto(RoleBo bo)
        {
            if (bo == null)
                return null;

            RoleDto dto = new RoleDto();
            dto.Id = bo.Id;
            dto.RoleName = bo.RoleName;
            dto.Desc = bo.Desc;
            dto.Grand = bo.GrandRole.Select(x => GrandBo.ConvertToDto(x.Grand)).ToList();

            return dto;
        }

        public static RoleBo ConvertToBusinessObject(RoleDto dto)
        {
            if (dto == null)
                return null;

            RoleBo bo = new RoleBo();
            bo.Id = dto.Id;
            bo.RoleName = dto.RoleName;
            bo.Desc = dto.Desc;
            //bo.ActionTypeRole = dto.ActionType.Select(x => ActionTypeBo.ConvertToBusinessObject(x.ActionType)).ToList();

            return bo;
        }
    }
}
