using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Web.Filters
{
    public class AuthorizeRaporActionFilter : IAsyncActionFilter
    {
        private readonly HttpContext _httpContext;

        public AuthorizeRaporActionFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Claim birimAmiriClaim = _httpContext.User.Claims.FirstOrDefault(x => x.Type == "BirimAmiri");

            bool birimAmiri = birimAmiriClaim != null && birimAmiriClaim.Value == "true";

            if (birimAmiri)
            {
                await next();
            }
            else
            {
                _httpContext.Response.StatusCode = 403;
            }
        }
    }
}
