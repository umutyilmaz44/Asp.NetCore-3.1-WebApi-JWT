using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace base_app_webapi.Helper
{
    [Serializable]
    [DataContract]
    public class GenericResponse
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public ResultType ResultType { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public int? StatusCode { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        #region Constructors
        public GenericResponse(ResultType resultType) : this((resultType == ResultType.Success), resultType, string.Empty, 0)
        {
        }

        public GenericResponse(int TotalCount) : this(true, ResultType.Success, string.Empty, TotalCount)
        {
        }

        public GenericResponse(string Message) : this(true, ResultType.Success, Message, 0)
        {
        }

        public GenericResponse(ResultType ResultType, string Message) : this(true, ResultType, Message, 0)
        {
        }
        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message) : this(IsSuccess, ResultType, Message, 0)
        {
        }

        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message, int TotalCount) : this(IsSuccess, ResultType, Message, TotalCount, "", null)
        {

        }

        public GenericResponse(ResultType ResultType, string Message, string errorCode, int? statusCode) :
            this((ResultType == ResultType.Success), ResultType, Message, 0, errorCode, statusCode)
        {

        }

        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message, int TotalCount, string errorCode, int? statusCode)
        {
            this.IsSuccess = IsSuccess;
            this.ResultType = ResultType;
            this.Message = Message;            
            this.TotalCount = TotalCount;
            this.ErrorCode = errorCode;
            this.StatusCode = statusCode;
        }
        #endregion
    
        #region Return Methods
        public static GenericResponse Ok(){
            return new GenericResponse(ResultType.Success);
        }
        public static GenericResponse Error(ResultType resultType, string message, string errorCode, int? statusCode){
            return new GenericResponse(resultType, message, errorCode, statusCode);
        }
        #endregion
    }

    [Serializable]
    [DataContract]
    public class GenericResponse<T>
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public ResultType ResultType { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public int? StatusCode { get; set; }

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public T Data { get; set; }

        #region Constructors
        private GenericResponse()
        {
        }

        private GenericResponse(ResultType resultType) : this((resultType == ResultType.Success), resultType, string.Empty, default(T), 0)
        {
        }

        public GenericResponse(T Data) : this(true, ResultType.Success, string.Empty, Data, 0)
        {
        }

        public GenericResponse(T Data, int TotalCount) : this(true, ResultType.Success, string.Empty, Data, TotalCount)
        {
        }

        public GenericResponse(T Data, string Message) : this(true, ResultType.Success, Message, Data, 0)
        {
        }

        public GenericResponse(ResultType ResultType, string Message) : this(true, ResultType, Message, default(T), 0)
        {
        }
        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message) : this(IsSuccess, ResultType, Message, default(T), 0)
        {
        }

        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message, T Data) : this(IsSuccess, ResultType, Message, Data, 0)
        {
        }
        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message, T Data, int TotalCount) : this(IsSuccess, ResultType, Message, Data, TotalCount, "", null)
        { 
        
        }

        public GenericResponse(ResultType ResultType, string Message, string errorCode, int? statusCode) : 
            this((ResultType == ResultType.Success), ResultType, Message, default(T), 0, errorCode, statusCode)
        {

        }

        public GenericResponse(bool IsSuccess, ResultType ResultType, string Message, T Data, int TotalCount, string errorCode, int? statusCode)
        {
            this.IsSuccess = IsSuccess;
            this.ResultType = ResultType;
            this.Message = Message;
            this.Data = Data;
            this.TotalCount = TotalCount;
            this.ErrorCode = errorCode;
            this.StatusCode = statusCode;
        }
        #endregion
    
        #region Return Methods
        public static GenericResponse<T> Ok(){
            return new GenericResponse<T>(true,ResultType.Success, "", default(T));
        }
        public static GenericResponse<T> Ok(T value){
            return new GenericResponse<T>(true,ResultType.Success, "", value);
        }
        public static GenericResponse<T> Error(ResultType resultType, string message, string errorCode, int? statusCode){
            return new GenericResponse<T>(resultType, message, errorCode, statusCode);
        }
        #endregion
    }

    public enum ResultType : int
    {
        Information = 1,
        Validation = 2,
        Success = 3,
        Warning = 4,
        Error = 5
    };
}