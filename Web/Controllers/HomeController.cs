using Application.Features.Izinler.Queries.GetIslemYapilanIzinler;
using Application.Features.Izinler.Queries.GetKalanSenelikIzinGunSayisiByCalisanId;
using Application.Features.Izinler.Queries.GetOnaylanacakIzinler;
using Application.Features.Izinler.Queries.GetSuankiIzinByCalisanId;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : UserController
    {
        public async Task<IActionResult> Index()
        {
            GetSuankiIzinByCalisanIdQuery getSuankiIzinByCalisanIdQuery = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            GetSuankiIzinByCalisanIdResponse getSuankiIzinByCalisanIdResponse = await Mediator.Send(getSuankiIzinByCalisanIdQuery);

            string role = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            int gelenIzinTalepSayisi = default;
            int islemYapilanIzinSayisi = default;

            if (role == "Onay_Yetkilisi" || role == "Yetkili")
            {
                GetOnaylanacakIzinlerQuery getOnaylanacakIzinlerQuery = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
                var onayBekleyenIzinler = await Mediator.Send(getOnaylanacakIzinlerQuery);
                gelenIzinTalepSayisi = onayBekleyenIzinler.Count;

                GetIslemYapilanIzinlerQuery getIslemYapilanIzinlerQuery = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
                var islemYapilanIzinler = await Mediator.Send(getIslemYapilanIzinlerQuery);
                islemYapilanIzinSayisi = islemYapilanIzinler.Count;
            }

            GetKalanSenelikIzinGunSayisiByCalisanIdQuery getKalanSenelikIzinGunSayisiByCalisanIdQuery = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            var kalanSenelikIzinGunSayisiResponse = await Mediator.Send(getKalanSenelikIzinGunSayisiByCalisanIdQuery);

            HomeVM homeVM = new()
            {
                BaslangicTarihi = getSuankiIzinByCalisanIdResponse?.BaslangicTarihi,
                BitisTarihi = getSuankiIzinByCalisanIdResponse?.BitisTarihi,
                IseBaslamaTarihi = getSuankiIzinByCalisanIdResponse?.IseBaslamaTarihi,
                GunSayisi = getSuankiIzinByCalisanIdResponse?.GunSayisi ?? 0,
                KalanSenelikIzinGunSayisi = kalanSenelikIzinGunSayisiResponse.KalanSenelikIzinGunSayisi,
                GelenIzinTalepSayisi = gelenIzinTalepSayisi,
                IslemYapilanIzinSayisi = islemYapilanIzinSayisi,
            };

            return View(homeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}