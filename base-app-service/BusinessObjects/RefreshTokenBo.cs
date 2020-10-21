using base_app_common.dto.refreshtoken;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_service.Bo
{
    public class RefreshTokenBo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual UserBo User { get; set; }

        public static RefreshTokenBo ConvertToBusinessObject(RefreshTokenDto dto)
        {
            if (dto == null)
                return null;

            RefreshTokenBo bo = new RefreshTokenBo();
            bo.Id = dto.Id;
            bo.ExpiryDate = dto.ExpiryDate;
            bo.Token = dto.Token;
            bo.UserId = dto.UserId;
            bo.User = UserBo.ConvertToBusinessObject(dto.User);

            return bo;
        }

        public static RefreshTokenDto ConvertToDto(RefreshTokenBo bo)
        {
            if (bo == null)
                return null;

            RefreshTokenDto dto = new RefreshTokenDto();
            dto.Id = bo.Id;
            dto.ExpiryDate = bo.ExpiryDate;
            dto.Token = bo.Token;
            dto.UserId = bo.UserId;
            dto.User = UserBo.ConvertToDto(bo.User);

            return dto;
        }
    }
}
