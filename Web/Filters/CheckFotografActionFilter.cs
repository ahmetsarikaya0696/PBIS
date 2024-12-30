using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Web.Filters
{
    public class CheckFotografActionFilter : IAsyncActionFilter
    {
        private readonly HttpContext _httpContext;
        private readonly IBaskentResimBlobRepository _baskentResimBlobRepository;

        public CheckFotografActionFilter(IHttpContextAccessor httpContextAccessor, IBaskentResimBlobRepository baskentResimBlobRepository)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _baskentResimBlobRepository = baskentResimBlobRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Claim claim = _httpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id");
            bool isLoggedIn = claim?.Value != null; // claim != null && claim.value != null kodu ile eşdeğer

            if (isLoggedIn && _httpContext.Session.GetString("user_image") == null)
            {
                var LREF = _httpContext.User.Claims.FirstOrDefault(x => x.Type == "LREF").Value;

                byte[] fotoByteArray = (await _baskentResimBlobRepository.GetAsync(predicate: x => x.LREF.ToString() == LREF)).RESIM;
                string fotoBase64 = Convert.ToBase64String(fotoByteArray);

                _httpContext.Session.SetString("user_image", fotoBase64);
            }

            await next();
        }
    }
}
