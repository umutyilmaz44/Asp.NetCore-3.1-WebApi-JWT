using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace base_app_service.Bo
{
    public class PageBo
    {
        public PageBo()
        {           
        }

        public long Id { get; set; }
        public string PageName { get; set; }
        public string NaviagteUrl { get; set; }
    }
}
