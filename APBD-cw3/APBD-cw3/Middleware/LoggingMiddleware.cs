using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBD_cw3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Time: " + DateTime.Now + "\n").Append("Method: " + 
                httpContext.Request.Method + "\n").Append("Request: " + 
                httpContext.Request.Path + "\n");
            var bodyStr = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync();
            }
            stringBuilder.Append(bodyStr + "\n").Append(
                httpContext.Request.Query + "\n").Append(
                ("---------\n"));
            File.AppendAllText("log.txt", stringBuilder.ToString());
            await _next(httpContext);
        }
    }
}
