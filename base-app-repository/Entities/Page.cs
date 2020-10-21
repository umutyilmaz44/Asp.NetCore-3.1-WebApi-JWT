using System;
using System.Collections.Generic;

namespace base_app_repository.Entities
{
    public partial class Page
    {
        public long Id { get; set; }
        public string PageName { get; set; }
        public string NaviagteUrl { get; set; }
    }
}
