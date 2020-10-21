using base_app_common.dto.grand;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_common.dto.page
{
    public class PageDto
    {
        public PageDto()
        {           
        }

        public long Id { get; set; }
        public string PageName { get; set; }
        public string NaviagteUrl { get; set; }
        public long ActionTypeId { get; set; }
        
        public virtual GrandDto Grand { get; set; }

    }
}
