using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common
{
    public class FilterCriteria
    {
        public FilterCriteria()
        {
            QueryFilter = "";
            IncludeProperties = "";
            PagingFilter = new PagingFilter();
            QueryFilterParameters = new object[0];
            DictonaryBasedFilter = new List<DictonaryFilter>();
        }

        public PagingFilter PagingFilter { get; set; }
        public string QueryFilter { get; set; }
        public object[] QueryFilterParameters { get; set; }
        public string OrderFilter { get; set; }
        public string IncludeProperties { get; set; }
        
        public List<DictonaryFilter> DictonaryBasedFilter { get; set; }
    }

    public class DictonaryFilter
    {
        public string Key { get; set; }
        public string Data { get; set; }
        public OperandType OperandType { get; set; }
    }

    public enum OperandType
    {
        None=0,
        Like = 1,
        Equal = 2,
        NotEqual = 3,
        Greater = 4,
        GreaterThan = 5,
        Less = 6,
        LessThan = 7
    }
}
