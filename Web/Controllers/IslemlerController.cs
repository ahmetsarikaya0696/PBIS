using Application.Features.IzinGruplari.Queries.GetAllIzinGrup;
using Application.Features.IzinHareketleri.Queries.GetIzinHareketleriByIzinId;
using Application.Features.Izinler.Commands.Create;
using Application.Features.Izinler.Commands.IptalEt;
using Application.Features.Izinler.Commands.Update;
using Application.Features.Izinler.Queries.GetBitisTarihi;
using Application.Features.Izinler.Queries.GetByCalisanId;
using Application.Features.Izinler.Queries.GetGunSayisiVeIseBaslamaTarihi;
using Application.Features.Izinler.Queries.GetIzinFormTabVerileriById;
using Application.Features.Izinler.Queries.GetIzinFormVerileriById;
using Application.Features.IzinTurleri.Queries.GetAllIzinTurSelectList;
using Application.Features.IzinTurleri.Queries.GetIzinTurById;
using Application.Features.RetSebepleri.Queries.GetAllRetSebepExtended;
using Application.Features.RetSebepleri.Queries.GetRetDetayByIzinHareketId;
using Application.Features.RetSebepleri.Queries.GetRetDetayByIzinId;
using Application.Features.Tatiller.Queries.GetTatiller;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Filters;
using Web.Models;

namespace Web.Controllers
{
    [ServiceFilter(typeof(CheckFotografActionFilter))]
    public class IslemlerController : UserController
    {
        private readonly IMapper _mapper;

        public IslemlerController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("/islemler/izin")]
        public async Task<IActionResult> Izin(IzinlerimVM izinlerimVM)
        {
            ViewBag.IzinTurleri = await GetIzinTurleriAsync();

            return View(izinlerimVM);
        }

        [HttpPost("/islemler/izin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzinTalebiOlustur(IzinlerimVM izinlerimVM)
        {
            CreateIzinCommand command = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
            };

            if (izinlerimVM.FormTipIzinCreateVM != null)
                _mapper.Map(izinlerimVM.FormTipIzinCreateVM, command);
            else
                _mapper.Map(izinlerimVM.TextTipIzinCreateVM, command);

            CreatedIzinResponse response = await Mediator.Send(command);

            return new JsonResult(response);
        }

        [HttpPost("/islemler/iptal/izin/{izinId}"), ValidateAntiForgeryToken]
        public async Task<JsonResult> IzinTalebiniIptalEt(int izinId)
        {
            IptalEtIzinCommand command = new() { IzinId = izinId };
            bool iptalEdildi = await Mediator.Send(command);

            return new JsonResult(iptalEdildi);
        }

        [HttpPost("/islemler/duzenle/izin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzniDuzenle(IzinlerimVM izinlerimVM)
        {
            UpdateIzinCommand command = new()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
            };

            if (izinlerimVM.FormTipIzinUpdateVM != null)
                _mapper.Map(izinlerimVM.FormTipIzinUpdateVM, command);
            else
                _mapper.Map(izinlerimVM.TextTipIzinUpdateVM, command);

            bool guncellemeBasarili = await Mediator.Send(command);

            return new JsonResult(guncellemeBasarili);
        }

        [HttpGet("/izinlerim")]
        public async Task<IActionResult> GetKullanıcıIzinleriAsync()
        {
            GetByCalisanIdIzinQuery getByCalisanIdIkIzinQuery = new() { CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            List<GetByCalisanIdIzinResponse> response = await Mediator.Send(getByCalisanIdIkIzinQuery);
            return new JsonResult(new { data = response });
        }

        [HttpGet("/izinhareketleri/izin/{izinId}")]
        public async Task<IActionResult> GetIzinHareketleriByIzinId(int izinId)
        {
            GetIzinHareketleriByIzinIdQuery query = new() { IzinId = izinId, CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value) };
            List<GetIzinHareketleriByIzinIdResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }


        [HttpGet("/izin-form-tab-verileri/izin/{izinId}")]
        public async Task<IActionResult> GetIzinFormTabVerileriByIzinId(int izinId)
        {
            GetIzinFormTabVerileriByIdQuery query = new() { Id = izinId };
            GetIzinFormTabVerileriByIdResponse response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("/izin-form-verileri/izin/{izinId}")]
        public async Task<IActionResult> GetIzinFormVerileriByIzinId(int izinId)
        {
            GetIzinFormVerileriByIdQuery query = new() { Id = izinId };
            GetIzinFormVerileriByIdResponse response = await Mediator.Send(query);

            return new JsonResult(response);
        }

        [HttpGet("/izin-tarihleri")]
        public async Task<IActionResult> GetGunSayisiVeIseBaslamaTarihi(string baslangicTarihi, string bitisTarihi, string mahsubenBaslangicTarihi, int izinTurId)
        {
            GetGunSayisiVeIseBaslamaTarihiQuery getIzinBitisVeIseBaslamaTarihiQuery = new()
            {
                IzinTurId = izinTurId,
                BaslangicTarihi = Convert.ToDateTime(baslangicTarihi),
                BitisTarihi = Convert.ToDateTime(bitisTarihi),
                MahsubenBaslangicTarihi = mahsubenBaslangicTarihi != null ? Convert.ToDateTime(mahsubenBaslangicTarihi) : null
            };

            GetGunSayisiVeIseBaslamaTarihiResponse response = await Mediator.Send(getIzinBitisVeIseBaslamaTarihiQuery);

            return new JsonResult(response);
        }

        [HttpGet("/izin-bitis-tarihi")]
        public async Task<IActionResult> GetBitisTarihi(string baslangicTarihi, int izinTurId)
        {
            GetBitisTarihiQuery query = new()
            {
                BaslangicTarihi = Convert.ToDateTime(baslangicTarihi).Date,
                IzinTurId = izinTurId,
            };

            GetBitisTarihiResponse response = await Mediator.Send(query);

            return new JsonResult(response);
        }

        [HttpGet("/tatilgunleri")]
        public async Task<IActionResult> GetTatilGunleriAsync()
        {
            GetTatillerQuery query = new();
            List<GetTatillerResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpGet("/retsebepleri")]
        public async Task<IActionResult> GetRetSebepleriAsync()
        {
            GetAllRetSebepExtendedQuery query = new();
            List<GetAllRetSebepExtendedResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpGet("/islemler/izinturleri/id={id}")]
        public async Task<IActionResult> GetIzinTurById(int id)
        {
            GetIzinTurByIdQuery query = new() { Id = id };
            GetIzinTurByIdResponse response = await Mediator.Send(query);

            return new JsonResult(response);
        }

        [HttpGet("/ret-detay/izinId={izinId}")]
        public async Task<IActionResult> GetRetDetayByIzinId(int izinId)
        {
            GetRetDetayByIzinIdQuery query = new() { IzinId = izinId };
            List<GetRetDetayByIzinIdResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("/izingruplari")]
        public async Task<IActionResult> GetIzinGruplariAsync()
        {
            GetAllIzinGrupQuery query = new();
            List<GetAllIzinGrupResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpGet("/ret-detay")]
        public async Task<IActionResult> GetRetDetayByIzinHareketId(int izinHareketId)
        {
            GetRetDetayByIzinHareketIdQuery query = new() { Id = izinHareketId };
            GetRetDetayByIzinHareketIdResponse response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        private async Task<List<SelectListItem>> GetIzinTurleriAsync()
        {
            GetAllIzinTurSelectListQuery getByKullaniciAdiIkCalisanQuery = new();
            List<GetAllIzinTurSelectListResponse> response = await Mediator.Send(getByKullaniciAdiIkCalisanQuery);

            var list = response
                .Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Aciklama,
                        Value = x.Id.ToString(),
                        Selected = x.Aciklama == "Senelik"
                    })
                .OrderBy(x => x.Text)
                .ToList();
            return list;
        }
    }
}
