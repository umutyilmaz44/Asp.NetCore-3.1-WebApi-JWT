using System;
using System.Collections.Generic;
using System.Text;

namespace base_app_common
{
    public class ServiceResult<T>
    {
        public readonly bool Success;
        public readonly T Data;
        public readonly string Error;
        public PagingFilter paging;

        public ServiceResult(T data, bool success, string error = "", PagingFilter pagingFilter = null)
        {
            this.Data = data;
            this.Success = success;
            this.Error = error;
            this.paging = pagingFilter;
        }
    }
    public class ServiceResult
    {
        public readonly bool Success;
        public readonly string Error;

        public ServiceResult(bool success, string error = "")
        {
            this.Success = success;
            this.Error = error;
        }
    }
}
