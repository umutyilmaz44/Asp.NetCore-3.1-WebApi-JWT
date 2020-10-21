using base_app_common.dto.organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace base_app_service.Bo
{
    public class OrganizationBo
    {
        public OrganizationBo()
        {
            User = new HashSet<UserBo>();
        }

        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime RecordDate { get; set; }

        public virtual ICollection<UserBo> User { get; set; }

        public static OrganizationBo ConvertToBusinessObject(OrganizationDto dto)
        {
            if (dto == null)
                return null;

            OrganizationBo bo = new OrganizationBo();
            bo.Id = dto.Id;
            bo.Description = dto.Description;
            bo.ParentId = dto.ParentId;
            bo.RecordDate = dto.RecordDate;
            bo.Title = dto.Title;
            bo.User = dto.User.Select(x => UserBo.ConvertToBusinessObject(x)).ToList();

            return bo;
        }

        public static OrganizationDto ConvertToDto(OrganizationBo bo)
        {
            if (bo == null)
                return null;

            OrganizationDto dto = new OrganizationDto();
            dto.Id = bo.Id;
            dto.Description = bo.Description;
            dto.ParentId = bo.ParentId;
            dto.RecordDate = bo.RecordDate;
            dto.User = bo.User.Select(x => UserBo.ConvertToDto(x)).ToList();

            return dto;
        }
    }
}
