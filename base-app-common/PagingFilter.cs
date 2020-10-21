using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace base_app_common
{
    [Serializable]
    [DataContract]
    public class PagingFilter
    {
        [DataMember]
        public int pageNumber { get; set; } = 1;

        [DataMember]
        public int pageSize { get; set; } = 20;

        [DataMember]
        public int pageCount { get; set; } = 20;

        [DataMember]
        public int totalCount { get; set; }

        [DataMember]
        public int skipAmount { get { return pageSize * (pageNumber - 1); } }
    }
}
