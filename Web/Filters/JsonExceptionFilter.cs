using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is ClientsideException)
            {
                context.Result = new JsonResult(new
                {
                    ErrorMessage = exception.Message,
                });

                context.ExceptionHandled = true;
            }
        }
    }
}
