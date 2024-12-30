using Application.Features.Dogrulamalar.Commands.Create;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class DogrulamaController : UserController
    {
        // Doğrulama oluştur ve doğrulama kodunu gönder
        [HttpPost("/dogrulama/izin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Dogrulama(string yontem)
        {
            CreateDogrulamaCommand command = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value), Yontem = yontem };
            CreatedDogrulamaResponse response = await Mediator.Send(command);

            return new JsonResult(response);
        }
    }
}
