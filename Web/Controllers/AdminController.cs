using Application.Exceptions;
using Application.Features.Birimler.Queries.GetBirimlerSelectList;
using Application.Features.Calisanlar.Queries.GetByIzinOnayTanimId;
using Application.Features.Calisanlar.Queries.GetLikeAdSoyad;
using Application.Features.Isyerleri.Queries.GetIsyerleriSelectList;
using Application.Features.IzinGruplari.Commands.Create;
using Application.Features.IzinGruplari.Commands.Delete;
using Application.Features.IzinGruplari.Commands.Update;
using Application.Features.IzinOnayTanimlari.Commands.Create;
using Application.Features.IzinOnayTanimlari.Commands.Delete;
using Application.Features.IzinOnayTanimlari.Commands.Update;
using Application.Features.IzinOnayTanimlari.Queries.GetAllIzinOnayTanim;
using Application.Features.IzinOnayTanimlari.Queries.GetIzinOnayTanimAdVeSiraByIzinGrupId;
using Application.Features.IzinOnayTanimlari.Queries.GetIzinOnayTanimSelectListItems;
using Application.Features.IzinTurleri.Commands.Create;
using Application.Features.IzinTurleri.Commands.Delete;
using Application.Features.IzinTurleri.Commands.Update;
using Application.Features.IzinTurleri.Queries.GetAllIzinTur;
using Application.Features.OrganizasyonHareketleri.Queries.GetOrganizasyonHareketleriByOrganizasyonId;
using Application.Features.Organizasyonlar.Commands.DeleteOrganizasyon;
using Application.Features.Organizasyonlar.Commands.UpdateOrganizasyon;
using Application.Features.Organizasyonlar.Queries.GetOrganizasyonById;
using Application.Features.Organizasyonlar.Queries.GetOrganizasyonSelectListWithTabAndMenu;
using Application.Features.OrganizasyonSemasi.Commands.CreateOrganizasyon;
using Application.Features.OrganizasyonSemasi.Queries.GetAnaBirimler;
using Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonlar;
using Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonSemasi;
using Application.Features.RetSebepleri.Commands.Create;
using Application.Features.RetSebepleri.Commands.Delete;
using Application.Features.RetSebepleri.Commands.Update;
using Application.Features.RetSebepleri.Queries.GetRetSebepleriByAciklama;
using Application.Features.Tatiller.Commands.Create;
using Application.Features.Tatiller.Commands.Delete;
using Application.Features.Tatiller.Commands.Update;
using Application.Features.Unvanlar.Queries.GetUnvanlarSelectList;
using Application.Notifications.OrganizasyonHareketleri;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Filters;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles = "Yetkili")]
    [ServiceFilter(typeof(CheckFotografActionFilter))]
    public class AdminController : BaseController
    {
        private readonly IMapper _mapper;

        public AdminController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("admin/tatilgunleri")]
        public IActionResult TatilGunleri()
        {
            return View();
        }

        [HttpPost("admin/tatilgunleri"), ValidateAntiForgeryToken]
        public async Task<IActionResult> TatilGunleri(CreateTatilVM createTatilVM)
        {
            try
            {
                CreateTatilCommand command = _mapper.Map<CreateTatilCommand>(createTatilVM);
                CreatedTatilResponse response = await Mediator.Send(command);
                TempData["success"] = $"\"{response.Tarih}\" tarihli tatil günü \"{response.Aciklama}\" açıklamasıyla başarıyla oluşturuldu!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = ex.Message;
            }

            return View();
        }

        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("/admin/tatilgunleri/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTatilGunuById(int id)
        {
            DeleteTatilByIdCommand command = new() { Id = id };
            DeletedTatilByIdResponse response = await Mediator.Send(command);
            return new JsonResult(response);
        }

        [HttpPost("admin/tatilgunleri/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTatilGunu(UpdateTatilVM updateTatilVM)
        {
            try
            {
                UpdateTatilCommand command = _mapper.Map<UpdateTatilCommand>(updateTatilVM);
                UpdatedTatilResponse response = await Mediator.Send(command);

                TempData["success"] = "Tatil günü başarıyla güncellendi!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = $"{ex.Message} Tatil günü güncellenemedi!";
            }

            return View(nameof(TatilGunleri));
        }

        [HttpGet("admin/retsebepleri")]
        public IActionResult RetSebepleri()
        {
            return View();
        }

        [HttpPost("admin/retsebepleri"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RetSebepleri(List<CreateRetSebepVM> createRetSebepVM)
        {
            GetRetSebepleriByAciklamaQuery getRetSebepleriByAciklamaQuery = new()
            {
                Aciklamalar = createRetSebepVM.Select(x => x.Aciklama).ToList()
            };

            List<string> uniqueOlmayanAciklamalar = await Mediator.Send(getRetSebepleriByAciklamaQuery);
            ViewBag.AlertDanger = uniqueOlmayanAciklamalar;

            var filteredEklenecekRetSebepleri = createRetSebepVM.Where(x => !uniqueOlmayanAciklamalar.Any(y => y.ToLower().Trim() == x.Aciklama.ToLower().Trim())).ToList();

            CreateRetSebepListCommand command = new()
            {
                CreateRetSebepCommands = _mapper.Map<List<CreateRetSebepCommand>>(filteredEklenecekRetSebepleri)
            };

            List<CreatedRetSebepResponse> response = await Mediator.Send(command);
            ViewBag.AlertSuccess = _mapper.Map<List<CreateRetSebepVM>>(response);

            TempData[response?.Count > 0 ? "success" : "error"] = response?.Count > 0 ? $"{response.Count} adet ret sebebip başarıyla eklendi!" : "Hiçbir veri eklenemedi";

            return View();
        }
        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("admin/retsebepleri/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRetSebepById(int id)
        {
            DeleteRetSebepByIdCommand command = new() { Id = id };
            string silinenRetSebebiAciklamasi = await Mediator.Send(command);
            return new JsonResult($"\"{silinenRetSebebiAciklamasi}\" açıklamalı ret sebebi başarıyla silindi!");
        }

        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("admin/retsebepleri/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRetSebep(UpdateRetSebepVM updateRetSebepVM)
        {
            try
            {
                UpdateRetSebepCommand command = _mapper.Map<UpdateRetSebepCommand>(updateRetSebepVM);
                UpdatedRetSebepResponse response = await Mediator.Send(command);

                TempData["success"] = "Ret sebebi başarıyla güncellendi!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = $"{ex.Message} Ret sebebi güncellenemedi!";
            }

            return View(nameof(RetSebepleri));
        }

        [HttpGet("admin/izingruplari")]
        public async Task<IActionResult> IzinGruplari()
        {
            ViewBag.IzinOnayTanimlari = await GetAktifIzinOnayTanimSelectListAsync();
            return View();
        }

        [HttpPost("admin/izingruplari"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzinGruplari(CreateIzinGrupVM createIzinGrupVM)
        {

            ViewBag.IzinOnayTanimlari = await GetAktifIzinOnayTanimSelectListAsync();

            CreateIzinGrupWithIzinOnayTanimlariCommand createIzinGrupWithIzinOnayTanimlariCommand = _mapper.Map<CreateIzinGrupWithIzinOnayTanimlariCommand>(createIzinGrupVM);

            bool response = await Mediator.Send(createIzinGrupWithIzinOnayTanimlariCommand);

            TempData[response ? "success" : "error"] = response ? $"İzin grubu başarıyla oluşturuldu!" : "İzin grubu oluşturken bir hata meydana geldi!";

            return View();
        }

        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("admin/izingruplari/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIzinGrupById(int id)
        {
            DeleteIzinGrupByIdCommand command = new() { Id = id };
            string response = await Mediator.Send(command);
            return new JsonResult(response);
        }

        [HttpPost("admin/izingruplari/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIzinGrup(UpdateIzinGrupVM updateIzinGrupVM)
        {
            try
            {
                UpdateIzinGrupCommand command = _mapper.Map<UpdateIzinGrupCommand>(updateIzinGrupVM);

                UpdatedIzinGrupResponse response = await Mediator.Send(command);

                ViewBag.IzinOnayTanimlari = await GetAktifIzinOnayTanimSelectListAsync();

                TempData["success"] = $"\"{response.Aciklama}\" açıklamalı izin grubu başarıyla güncellendi!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = $"{ex.Message} Güncelleme işlemi başarısız oldu!";
            }

            return View(nameof(IzinGruplari));
        }

        [HttpGet("admin/calisanlar")]
        public async Task<IActionResult> GetCalisanlarSelectListItemsAsync(string search)
        {
            GetLikeAdSoyadQuery query = new() { Search = search };
            List<GetLikeAdSoyadResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/calisanlar/izinonaytanim/{izinOnayTanimId}")]
        public async Task<IActionResult> GetCalisanlarSelectListItemsByIzinOnayTanimIdAsync(int izinOnayTanimId)
        {
            GetByIzinOnayTanimIdQuery query = new() { IzinOnayTanimId = izinOnayTanimId };
            List<GetByIzinOnayTanimIdResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/birim-list")]
        public async Task<IActionResult> GetBirimlerSelectListItemsAsync(string search)
        {
            GetBirimlerSelectListQuery query = new() { Search = search };
            List<GetBirimlerSelectListResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/isyerleri")]
        public async Task<IActionResult> GetIsyerleriSelectListItemsAsync(string search)
        {
            GetIsyerleriSelectListQuery query = new() { Search = search };
            List<GetIsyerleriSelectListResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/unvanlar")]
        public async Task<IActionResult> GetUnvanlarSelectListItemsAsync(string search)
        {
            GetUnvanlarSelectListQuery query = new() { Search = search };
            List<GetUnvanlarSelectListResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/izinonaytanimlari")]
        public IActionResult IzinOnayTanimlari()
        {
            return View();
        }

        [HttpGet("admin/izin-onay-tanimlari")]
        public async Task<IActionResult> GetIzinOnayTanimlariAsync()
        {
            GetAllIzinOnayTanimQuery query = new();
            List<GetAllIzinOnayTanimResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpPost("admin/izinonaytanimlari"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzinOnayTanimlari(CreateIzinOnayTanimVM createIzinOnayTanimVM)
        {
            try
            {
                CreateIzinOnayTanimWithCalisanlarCommand command = _mapper.Map<CreateIzinOnayTanimWithCalisanlarCommand>(createIzinOnayTanimVM);
                CreatedIzinOnayTanimWithCalisanlarResponse response = await Mediator.Send(command);

                string isAktif = response.Aktif ? "aktif" : "pasif";

                TempData["success"] = $"\"{response.Aciklama}\" açıklaması ile bir izin onay tanımı \"{isAktif}\" olarak oluşturuldu.";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = ex.Message;
            }

            return View();
        }

        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("admin/izinonaytanimlari/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIzinOnayTanimById(int id)
        {
            DeleteIzinOnayTanimByIdCommand command = new() { Id = id };
            string silinenIzinOnayTanimAciklamasi = await Mediator.Send(command);
            return new JsonResult($"\"{silinenIzinOnayTanimAciklamasi}\" açıklamalı izin onay tanımı başarıyla silindi!");
        }

        [HttpPost("admin/izinonaytanimlari/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIzinOnayTanim(UpdateIzinOnayTanimVM updateIzinOnayTanimVM)
        {
            try
            {
                UpdateIzinOnayTanimWithCalisanlarCommand command = _mapper.Map<UpdateIzinOnayTanimWithCalisanlarCommand>(updateIzinOnayTanimVM);
                UpdatedIzinOnayTanimWithCalisanlarResponse response = await Mediator.Send(command);

                TempData["success"] = "İzin onay tanımı başarıyla güncellendi!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = $"{ex.Message} Güncelleme başarısız!";
            }

            return View(nameof(IzinOnayTanimlari));
        }

        [HttpGet("admin/izinturleri")]
        public IActionResult IzinTurleri()
        {
            return View();
        }

        [HttpPost("admin/izinturleri"), ValidateAntiForgeryToken]
        public async Task<IActionResult> IzinTurleri(CreateIzinTurVM createIzinTurVM)
        {
            try
            {
                CreateIzinTurCommand createIzinTurCommand = _mapper.Map<CreateIzinTurCommand>(createIzinTurVM);

                CreatedIzinTurResponse response = await Mediator.Send(createIzinTurCommand);

                string isAktif = response.Aktif ? "aktif" : "pasif";

                TempData["success"] = $"\"{response.Aciklama}\" açıklaması ile bir izin türü \"{isAktif}\" olarak oluşturuldu.";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = ex.Message;
            }

            return View();
        }

        [HttpPost("admin/izinturleri/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIzinTur(UpdateIzinTurVM updateIzinTurVM)
        {
            try
            {
                UpdateIzinTurCommand command = _mapper.Map<UpdateIzinTurCommand>(updateIzinTurVM);
                UpdatedIzinTurResponse response = await Mediator.Send(command);

                TempData["success"] = $"İzin türü \"{response.Aciklama}\" açıklaması ile \"{response.Aktif}\" olarak güncellendi!";
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = ex.Message;
            }

            return View(nameof(IzinTurleri));
        }

        [ServiceFilter(typeof(JsonExceptionFilter))]
        [HttpPost("admin/izinturleri/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIzinTurById(int id)
        {
            DeleteIzinTurByIdCommand command = new() { Id = id };
            string silinenIzinTurAciklamasi = await Mediator.Send(command);
            return new JsonResult($"\"{silinenIzinTurAciklamasi}\" açıklamalı izin türü başarıyla silindi!");
        }

        [HttpGet("admin/izin-turleri")]
        public async Task<IActionResult> GetIzinTurleriAsync()
        {
            GetAllIzinTurQuery query = new();
            List<GetAllIzinTurResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpGet("admin/izinonaytanim")]
        public async Task<IActionResult> GetIzinOnayTanimVeSiraByIzinGrupId(int izinGrupId)
        {
            GetIzinOnayTanimAdVeSiraByIzinGrupIdQuery query = new() { IzinGrupId = izinGrupId };
            List<GetIzinOnayTanimAdVeSiraByIzinGrupIdResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/organizasyon-sema")]
        public async Task<IActionResult> GetOrganizasyonSema(int birimId)
        {
            GetOrganizasyonSemasiQuery query = new() { BirimId = birimId };
            List<GetOrganizasyonSemasiByBirimIdResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpGet("admin/organizasyon-semasi-islemleri")]
        public async Task<IActionResult> OrganizasyonSemaIslemleri()
        {
            GetOrganizasyonSelectListWithTabAndMenuQuery query = new();
            GetOrganizasyonSelectListWithTabAndMenuResponse response = await Mediator.Send(query);

            ViewBag.selectList = response.SelectListResponse.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList();

            ViewBag.menu = response.MenuResponse;

            return View();
        }

        [HttpGet("admin/organizasyonlar")]
        public async Task<IActionResult> GetOrganizasyonlar()
        {
            GetOrganizasyonlarQuery query = new();
            List<GetOrganizasyonlarResponse> response = await Mediator.Send(query);
            return new JsonResult(new { data = response });
        }

        [HttpPost("admin/organizasyonlar/olustur"), ValidateAntiForgeryToken]
        public async Task<IActionResult> OrganizasyonOlustur(CreateOrganizasyonVM createOrganizasyonVM)
        {
            try
            {
                CreateOrganizasyonCommand createOrganizasyonCommand = _mapper.Map<CreateOrganizasyonCommand>(createOrganizasyonVM);
                CreateOrganizasyonResponse response = await Mediator.Send(createOrganizasyonCommand);

                string aktif = response.Aktif ? "Aktif" : "Pasif";
                TempData["success"] = $"Bir kayıt \"{response.Aciklama_TR}\" açıklamasıyla \"{aktif}\" organizasyon şemasına eklendi";

                await Mediator.Publish(new OrganizasyonHareketNotification()
                {
                    CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                    Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    OrganizasyonId = response.Id,
                    IslemEnum = IslemEnum.Olusturuldu
                });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                throw;
            }
            return RedirectToAction(nameof(OrganizasyonSemaIslemleri));
        }

        [HttpPost("/admin/organizasyonlar/sil/{id}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> OrganizasyonSil(int id)
        {
            DeleteOrganizasyonByIdCommand command = new() { Id = id };
            DeleteOrganizasyonByIdResponse response = await Mediator.Send(command);

            await Mediator.Publish(new OrganizasyonHareketNotification()
            {
                CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                OrganizasyonId = response.Id,
                IslemEnum = IslemEnum.Silindi
            });

            return new JsonResult(response);
        }

        [HttpGet("admin/organizasyon-list")]
        public async Task<IActionResult> GetOrganizasyonlarSelectListItemsAsync(string search)
        {
            GetOrganizasyonlarSelectlistQuery query = new() { Search = search };
            List<GetOrganizasyonlarSelectlistResponse> response = await Mediator.Send(query);
            return new JsonResult(response);
        }


        [HttpGet("admin/ana-birimler")]
        public async Task<List<SelectListItem>> GetAnaBirimlerSelectlistAsync(string birimId)
        {
            GetOrganizasyonlarSelectlistQuery query = new();
            List<GetOrganizasyonlarSelectlistResponse> response = await Mediator.Send(query);
            var list = response.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Id,
                Selected = x.Id == birimId
            }).ToList();

            list.Insert(0, new SelectListItem("İzin onay tanımı seçiniz", "0", true, true));

            return list;
        }

        [HttpGet("admin/organizasyonlar/{id}")]
        public async Task<IActionResult> GetOrganizasyonById(int id)
        {
            GetOrganizasyonByIdQuery query = new() { Id = id };
            GetOrganizasyonByIdResponse response = await Mediator.Send(query);
            return new JsonResult(response);
        }

        [HttpPost("admin/organizasyon/guncelle"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrganizasyon(UpdateOrganizasyonVM updateOrganizasyonVM)
        {
            try
            {
                UpdateOrganizasyonCommand command = _mapper.Map<UpdateOrganizasyonCommand>(updateOrganizasyonVM);
                UpdatedOrganizasyonResponse response = await Mediator.Send(command);

                string aktif = response.Aktif ? "Aktif" : "Pasif";

                TempData["success"] = $"Kayıt \"{response.Aciklama_TR}\" açıklamasıyla \"{aktif}\" olarak güncellendi";

                await Mediator.Publish(new OrganizasyonHareketNotification()
                {
                    CalisanId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "Id").Value),
                    Ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    OrganizasyonId = response.Id,
                    IslemEnum = IslemEnum.Guncellendi
                });
            }
            catch (ClientsideException ex)
            {
                TempData["error"] = $"{ex.Message} birim güncellenemedi!";
            }

            return RedirectToAction(nameof(OrganizasyonSemaIslemleri));
        }

        [HttpGet("admin/organizasyon-hareketleri/{organizasyonId}")]
        public async Task<IActionResult> GetOrganizasyonHareketleriByOrganizasyonId(int organizasyonId)
        {
            GetOrganizasyonHareketleriByOrganizasyonIdQuery query = new() { OrganizasyonId = organizasyonId };
            List<GetOrganizasyonHareketleriByOrganizasyonIdResponse> response = await Mediator.Send(query);

            return new JsonResult(response);
        }

        private async Task<List<SelectListItem>> GetAktifIzinOnayTanimSelectListAsync()
        {
            GetIzinOnayTanimSelectListItemsQuery query = new();
            List<GetIzinOnayTanimSelectListItemsResponse> response = await Mediator.Send(query);

            var list = response
                .Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Aciklama,
                        Value = x.Id.ToString()
                    })
                .OrderBy(x => x.Text)
                .ToList();
            list.Insert(0, new SelectListItem("İzin onay tanımı seçiniz", "0", true, true));
            return list;
        }
    }
}