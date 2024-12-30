using Application.Features.Izinler.Queries.GetIzinByBirimId;
using Application.Features.Organizasyonlar.Queries.GetOrganizasyonSelectListWithTabAndMenu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Filters;

namespace Web.Controllers
{
    [ServiceFilter(typeof(AuthorizeRaporActionFilter))]
    public class RaporController : UserController
    {
        public async Task<IActionResult> Izin()
        {
            ViewBag.selectList = await GetMenuSelectList();

            return View();
        }

        [HttpGet("rapor/izinler")]
        public async Task<IActionResult> GetIzinlerByBirimId(int organizasyonId)
        {
            GetIzinByOrganizasyonIdQuery query = new() { OrganizasyonId = organizasyonId };
            List<GetIzinByOrganizasyonIdResponse> response = await Mediator.Send(query);

            return new JsonResult(new { data = response });
        }

        private async Task<List<SelectListItem>> GetMenuSelectList()
        {
            GetOrganizasyonSelectListWithTabAndMenuQuery query = new() { Kod = HttpContext.User.Claims.First(x => x.Type == "Kod").Value };
            GetOrganizasyonSelectListWithTabAndMenuResponse response = await Mediator.Send(query);

            return response.SelectListResponse.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList();
        }
    }
}
