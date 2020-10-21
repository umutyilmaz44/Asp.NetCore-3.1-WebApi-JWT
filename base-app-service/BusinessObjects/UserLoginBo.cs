using base_app_common.dto.user;
using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_service.Bo
{
    public class UserLoginBo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LoginTime { get; set; }

        public virtual UserBo User { get; set; }

        public static UserLoginDto ConvertToDto(UserLoginBo bo)
        {
            if (bo == null)
                return null;

            UserLoginDto dto = new UserLoginDto();
            dto.Id = bo.Id;
            dto.LoginTime = bo.LoginTime;
            dto.UserId = bo.UserId;
            dto.User = UserBo.ConvertToDto(bo.User);

            return dto;
        }

        public static UserLoginBo ConvertToBusinessObject(UserLoginDto dto)
        {
            if (dto == null)
                return null;

            UserLoginBo bo = new UserLoginBo();
            bo.Id = dto.Id;
            bo.LoginTime = dto.LoginTime;
            bo.UserId = dto.UserId;
            bo.User = UserBo.ConvertToBusinessObject(dto.User);

            return bo;
        }
    }
}
