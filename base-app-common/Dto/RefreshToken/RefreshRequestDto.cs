using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.refreshtoken
{
    public class RefreshRequestDto
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }
}
