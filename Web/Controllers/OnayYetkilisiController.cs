using Application.Features.Calisanlar.Queries.GetCalisanWithInfoByYetkiliId;
using Application.Features.IzinGruplari.Queries.GetAllIzinGrupByCalisanId;
using Application.Features.Izinler.Commands.Onayla;
using Application.Features.Izinler.Commands.Reddet;
using Application.Features.Izinler.Queries.GetIslemYapilanIzinler;
using Application.Features.Izinler.Queries.GetIzinDetayById;
using Application.Features.Izinler.Queries.GetOnaylanacakIzinler;
using Application.Features.RetSebepleri.Queries.GetAllRetSebep;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles = "Onay_Yetkilisi,Yetkili")]
    public class OnayYetkilisiController : BaseController
    {
        private readonly IMapper _mapper;

        public OnayYetkilisiController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("admin/islemler/izin-talepleri")]
        public IActionResult Izin()
        {
            return View();
        }

        [HttpGet("admin/islemler/izin-talepleri/islem-yapilanlar")]
        public IActionResult IslemYapilanIzinler()
        {
            return View();
        }

        [HttpGet("admin/islemler/izinler/islem-yapilanlar")]
        public async Task<IActionResult> GetIslemYapilanIzinlerAsync()
        {
            GetIslemYapilanIzinlerQuery query = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            List<GetIslemYapilanIzinlerResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpGet("/admin/islemler/izinler/islem-bekleyen")]
        public async Task<IActionResult> GetOnaylanacakIzinlerAsync()
        {
            GetOnaylanacakIzinlerQuery query = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            List<GetOnaylanacakIzinlerResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpPost("/islemler/izin/onayla/{izinId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzniOnayla(int izinId)
        {
            OnaylaIzinCommand approveIzinCommand = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                IzinId = izinId
            };

            OnaylaIzinResponse response = await Mediator.Send(approveIzinCommand);

            return RedirectToAction(nameof(Izin));
        }

        [HttpPost("/islemler/izin/reddet"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzniReddet(IzinReddetVM izinReddetVM)
        {
            ReddetIzinCommand command = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                IzinId = izinReddetVM.Id,
                Detay = izinReddetVM.Detay,
                RetSebepId = izinReddetVM.RetSebepId
            };

            ReddetIzinResponse response = await Mediator.Send(command);

            TempData["success"] = $"{response.IsyeriAdi} {response.BirimAdi} biriminde bulunan {response.AdSoyad} adlı çalışanın izin talebi başarıyla reddedildi!";

            return RedirectToAction(nameof(Izin));
        }

        [HttpGet("admin/islemler/izin-talepleri/{izinId}")]
        public async Task<IActionResult> IzinTalebiDetay(int izinId)
        {
            GetIzinDetayByIdQuery query = new() { Id = izinId };
            GetIzinDetayByIdResponse response = await Mediator.Send(query);


            ViewBag.RetSebepleri = await GetRetSebepleriAsync(response.PersonelSubeYetkisi);

            return View(_mapper.Map<IzinDetayVM>(response));
        }

        [HttpGet("/admin/izinler/{izinId}")]
        public async Task<IActionResult> GetIzinDetayByIdAsync(int izinId)
        {
            GetIzinDetayByIdQuery query = new() { Id = izinId };
            GetIzinDetayByIdResponse response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        private async Task<List<SelectListItem>> GetRetSebepleriAsync(bool personelSubeyeOzgu)
        {
            GetAllRetSebepQuery getAllRetSebepQuery = new() { PersonelSubeyeOzgu = personelSubeyeOzgu };
            List<GetAllRetSebepResponse> response = await Mediator.Send(getAllRetSebepQuery);

            var list = response
                .Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Aciklama,
                        Value = x.Id.ToString()
                    })
                .ToList();

            list.Insert(0, new SelectListItem("Ret sebebi seçiniz", "0", true, true));

            return list;
        }

        [HttpGet("admin/genel-izin-bilgileri")]
        public async Task<IActionResult> IzinBilgileri(int izinGrupId)
        {
            ViewBag.IzinGruplari = await GetIzinGruplariByCalisanIdAsync(izinGrupId);

            return View();
        }

        [HttpGet("admin/izin-bilgileri")]
        public async Task<JsonResult> IzinBilgileriJson(int? izinGrupId = null)
        {
            GetCalisanWithInfoByYetkiliIdQuery query = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                IzinGrupId = izinGrupId
            };

            List<GetCalisanWithInfoByYetkiliIdResponse> response = await Mediator.Send(query);

            return new JsonResult(new { data = response });
        }

        private async Task<List<SelectListItem>> GetIzinGruplariByCalisanIdAsync(int izinGrupId)
        {
            GetSelectListIzinGrupByCalisanIdQuery getSelectListIzinGrupByCalisanIdQuery = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value)
            };

            List<GetSelectListIzinGrupByCalisanIdResponse> response = await Mediator.Send(getSelectListIzinGrupByCalisanIdQuery);

            var list = response.Select(x => new SelectListItem()
            {
                Text = x.Aciklama,
                Value = x.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem("İzin grubu seçiniz", "", true));
            list.Insert(1, new SelectListItem("Tümünü Seç", "-1", false, false));

            var selectedItem = list.FirstOrDefault(x => x.Value == izinGrupId.ToString());
            if (selectedItem != null) selectedItem.Selected = true;

            return list;
        }
    }
}
