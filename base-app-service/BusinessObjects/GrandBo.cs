using base_app_common.dto.grand;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class GrandBo
    {
        public GrandBo()
        {
            GrandRole = new HashSet<GrandRoleBo>();
        }

        public long Id { get; set; }
        public string GrandName { get; set; }

        public virtual ICollection<GrandRoleBo> GrandRole { get; set; }

        public static GrandBo ConvertToBusinessObject(GrandBo dto)
        {
            if (dto == null)
                return null;

            GrandBo bio = new GrandBo();
            bio.Id = dto.Id;
            bio.GrandName = dto.GrandName;

            return bio;
        }

        public static GrandDto ConvertToDto(GrandBo bo)
        {
            if (bo == null)
                return null;

            GrandDto dto = new GrandDto();
            dto.Id = bo.Id;
            dto.GrandName = bo.GrandName;

            return dto;
        }
    }
}
