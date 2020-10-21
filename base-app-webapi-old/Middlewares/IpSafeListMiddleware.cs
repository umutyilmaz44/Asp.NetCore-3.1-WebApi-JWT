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
        private readonly ILogger _logger;
        private readonly string _safelist;

        public IpSafeListMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, string safelist)
        {
            _safelist = safelist;
            _next = next;
            _logger = loggerFactory.CreateLogger<IpSafeListMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Method != HttpMethod.Get.Method)
            //{
            
            string path = context.Request?.Path;
            path = string.IsNullOrEmpty(path) ? "" : path;

            IPAddress remoteIp = context.Connection.RemoteIpAddress;

            _logger.LogInformation(string.Format("Request from Client. IP address: {0} Path: {1}", remoteIp, path), Microsoft.Extensions.Logging.LogLevel.Information, "Middleware", "IpSafeListCheck");

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
                    _logger.LogWarning(string.Format("Forbidden Request from Client! IP address: {0} Path: {1}", remoteIp, path), Microsoft.Extensions.Logging.LogLevel.Warning, "Middleware", "IpSafeListCheck");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
            //}

            await _next.Invoke(context);
        }
    }
}
