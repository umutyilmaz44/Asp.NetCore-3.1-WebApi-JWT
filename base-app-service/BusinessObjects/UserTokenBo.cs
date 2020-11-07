using base_app_common.dto.refreshtoken;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_service.Bo
{
    public class UserTokenBo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLogout { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public virtual UserBo User { get; set; }

        public static UserTokenBo ConvertToBusinessObject(UserTokenDto dto)
        {
            if (dto == null)
                return null;

            UserTokenBo bo = new UserTokenBo();
            bo.Id = dto.Id;
            bo.UserId = dto.UserId;
            bo.AccessToken = dto.AccessToken;
            bo.RefreshToken = dto.RefreshToken;
            bo.ExpiryDate = dto.ExpiryDate;
            bo.IsLogout = dto.IsLogout;
            bo.LoginTime = dto.LoginTime;
            bo.LogoutTime = dto.LogoutTime;
            
            bo.User = UserBo.ConvertToBusinessObject(dto.User);

            return bo;
        }

        public static UserTokenDto ConvertToDto(UserTokenBo bo)
        {
            if (bo == null)
                return null;

            UserTokenDto dto = new UserTokenDto();
            dto.Id = bo.Id;
            dto.UserId = bo.UserId;
            dto.AccessToken = bo.AccessToken;
            dto.RefreshToken = bo.RefreshToken;
            dto.ExpiryDate = bo.ExpiryDate;
            dto.IsLogout = bo.IsLogout;
            dto.LoginTime = bo.LoginTime;
            dto.LogoutTime = bo.LogoutTime;
            dto.User = UserBo.ConvertToDto(bo.User);

            return dto;
        }
    }
}
