namespace Application.Services
{
    using Application.Features.Dogrulamalar.Commands.Create;
    using Application.Interfaces.Repositories;
    using Application.Interfaces.Services;
    using Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class DogrulamaService : IDogrulamaService
    {
        private readonly IDogrulamaRepository _dogrulamaRepository;
        private readonly ICalisanlarRepository _calisanlarRepository;
        private readonly ISmsService _smsService;
        private readonly IMailService _mailService;

        public DogrulamaService(IDogrulamaRepository dogrulamaRepository, ICalisanlarRepository calisanlarRepository, ISmsService smsService, IMailService mailService)
        {
            _dogrulamaRepository = dogrulamaRepository;
            _calisanlarRepository = calisanlarRepository;
            _smsService = smsService;
            _mailService = mailService;
        }

        public async Task<CreatedDogrulamaResponse> Gonder(int calisanId, string yontem, CancellationToken cancellationToken)
        {
            // Önceki dogrulamaları geçersiz yap
            List<Dogrulama> dogrulamalarByCalisanId = await _dogrulamaRepository.GetAllAsync(predicate: x => x.CalisanId == calisanId, cancellationToken: cancellationToken);
            if (dogrulamalarByCalisanId.Count > 0)
            {
                foreach (Dogrulama dogrulama in dogrulamalarByCalisanId)
                {
                    dogrulama.Gecerli = false;
                    await _dogrulamaRepository.UpdateAsync(dogrulama);
                }
            }

            // Yeni dogrulama olustur
            Dogrulama olusturulanDogrulama = await _dogrulamaRepository.AddAsync(new()
            {
                CalisanId = calisanId,
                Yontem = yontem,
            });

            string mesaj = $"TEST MESAJIDIR DİKKATE ALMAYIN. İZİN DOĞRULAMA KODU İzin talebiniz için doğrulama kodunuz : {olusturulanDogrulama.Kod}";

            Calisan calisan = await _calisanlarRepository.GetAsync(predicate: x => x.Id == calisanId, cancellationToken: cancellationToken);

            string tc = calisan.Tc.ToString();
            string iletisimBilgisi = string.Empty;

            if (yontem.ToLower() == "sms")
            {
                await _smsService.Send(mesaj, tc);
                iletisimBilgisi = calisan.Telefon;
            }
            else if (yontem.ToLower() == "e-posta")
            {
                await _mailService.Send(mesaj, "İZİN DOĞRULAMA KODU", calisan.KullaniciAdi, $"{calisan.Ad} {calisan.Soyad}");
                iletisimBilgisi = calisan.KullaniciAdi;
            }

            return new CreatedDogrulamaResponse()
            {
                IletisimBilgisi = iletisimBilgisi,
                SonKullanimTarihi = olusturulanDogrulama.SonKullanimTarihi.ToString("MMM d, yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                Yontem = olusturulanDogrulama.Yontem
            };
        }
    }

}
