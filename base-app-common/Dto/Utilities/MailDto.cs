using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common.dto.utilities
{
    public class MailDto
    {
        public string[] recipients { get; set; }
        public string[] bccList { get; set; }
        public string[] ccList { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string[] attachments { get; set; }
    }
}
