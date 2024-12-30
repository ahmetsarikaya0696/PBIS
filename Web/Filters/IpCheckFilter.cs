using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters
{
    public class IpCheckFilter : IAsyncActionFilter
    {
        private readonly HashSet<string> _ipWhiteList = new() { "127.0.0.1", "::1", "10.1.1.11" };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            for (int i = 30; i <= 50; i++)
            {
                _ipWhiteList.Add($"10.1.0.{i}");
            }

            HttpContext httpContext = context.HttpContext;

            string ip = httpContext.Connection.RemoteIpAddress.ToString();

            bool inWhiteList = _ipWhiteList.Contains(ip);

            if (inWhiteList)
            {
                await next();
            }
            else
            {
                httpContext.Response.StatusCode = 403;
            }
        }
    }
}
