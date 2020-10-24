using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace base_app_webapi.Middlewares
{
    public class IpSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpSafeListMiddleware> _logger;
        private readonly string _safelist;

        public IpSafeListMiddleware(RequestDelegate next, ILogger<IpSafeListMiddleware> logger, string safelist)
        {
            _safelist = safelist;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Method != HttpMethod.Get.Method)
            //{
            
            string path = context.Request?.Path;
            path = string.IsNullOrEmpty(path) ? "" : path;

            IPAddress remoteIp = context.Connection.RemoteIpAddress;

            //_logger.Log(string.Format("Request from Client. IP address: {0} Path: {1}", remoteIp, path), Microsoft.Extensions.Logging.LogLevel.Information, "Middleware", "IpSafeListCheck");

            if (!string.IsNullOrEmpty(_safelist))
            {
                string[] ip = _safelist.Split(';');

                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in ip)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    string controllerName = "Middleware";
                    string actionName = "IpSafeListCheck";
                    string message = string.Format("Forbidden Request from Client! IP address: {0} Path: {1}", remoteIp, path);
                    _logger.LogWarning(string.Format("{0,-30}{1,-30}{2}", controllerName, actionName, message));
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
            //}

            await _next.Invoke(context);
        }
    }
}
