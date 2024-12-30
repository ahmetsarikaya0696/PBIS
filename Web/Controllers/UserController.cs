using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Filters;

namespace Web.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(CheckFotografActionFilter))]
    public abstract class UserController : BaseController
    {

    }
}
