using Application.Features.Calisanlar.Queries.GetByKullaniciAdi;
using Application.Features.Calisanlar.Queries.GetRoleByCalisanId;
using Application.Interfaces.External;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Filters;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ILdapAuthenticationService _ldapAuthenticationService;

        public AuthController(ILdapAuthenticationService ldapAuthenticationService)
        {
            _ldapAuthenticationService = ldapAuthenticationService;
        }

        [HttpGet("/giris")]
        [ServiceFilter(typeof(IpCheckFilter))]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/giris"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (string.IsNullOrWhiteSpace(loginVM.Username.Trim()) || string.IsNullOrWhiteSpace(loginVM.Password))
            {
                ModelState.AddModelError("", "Email ve şifre alanları zorunludur!");
                return View(loginVM);
            }

            var isAuthenticated = await _ldapAuthenticationService.ValidateCredentialsAsync(loginVM.Username, loginVM.Password);

            if (!isAuthenticated)
            {
                ModelState.AddModelError("", "Kullancı adı veya şifre hatalı!");
                return View(loginVM);
            }

            GetByKullaniciAdiCalisanQuery getByKullaniciAdiCalisanQuery = new() { KullaniciAdi = loginVM.Username };

            var getByKullaniciAdiCalisanResponse = await Mediator.Send(getByKullaniciAdiCalisanQuery);

            if (getByKullaniciAdiCalisanResponse == null)
            {
                ModelState.AddModelError("", "Kullancı adı veya şifre hatalı!");
                return View(loginVM);
            }

            GetRoleByCalisanIdQuery getRoleByCalisanIdQuery = new() { CalisanId = getByKullaniciAdiCalisanResponse.Id };
            string role = await Mediator.Send(getRoleByCalisanIdQuery);
            var claims = new List<Claim>
            {
                new Claim("Id", getByKullaniciAdiCalisanResponse.Id.ToString()),
                new Claim("Birim", getByKullaniciAdiCalisanResponse.Birim),
                new Claim("Kod", getByKullaniciAdiCalisanResponse.Kod),
                new Claim("BirimAmiri", getByKullaniciAdiCalisanResponse.BirimAmiri ? "true" : "false"),
                new Claim("Unvan", getByKullaniciAdiCalisanResponse.Unvan),
                new Claim("Ad", getByKullaniciAdiCalisanResponse.Ad),
                new Claim("Soyad", getByKullaniciAdiCalisanResponse.Soyad),
                new Claim("SicilNo", getByKullaniciAdiCalisanResponse.SicilNo.ToString()),
                new Claim("Telefon", getByKullaniciAdiCalisanResponse.Telefon),
                new Claim("KullaniciAdi", getByKullaniciAdiCalisanResponse.KullaniciAdi),
                new Claim("LREF", getByKullaniciAdiCalisanResponse.LREF.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(5)
                });

            TempData["success"] = "Giriş Başarılı";

            return RedirectToAction("Index", "Home");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}
