using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetIzinByBirimId
{
    public class GetIzinByOrganizasyonIdQuery : IRequest<List<GetIzinByOrganizasyonIdResponse>>
    {
        public int OrganizasyonId { get; set; }

        public class GetIzinByOrganizasyonIdQueryHandler : IRequestHandler<GetIzinByOrganizasyonIdQuery, List<GetIzinByOrganizasyonIdResponse>>
        {
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IOrganizasyonlarRepository _organizasyonlarRepository;
            private readonly IIzinTurleriRepository _izinTurleriRepository;
            private readonly ITatillerRepository _tatillerRepository;

            public GetIzinByOrganizasyonIdQueryHandler(ICalisanlarRepository calisanlarRepository, IOrganizasyonlarRepository organizasyonlarRepository, IIzinTurleriRepository izinTurleriRepository, ITatillerRepository tatillerRepository)
            {
                _calisanlarRepository = calisanlarRepository;
                _organizasyonlarRepository = organizasyonlarRepository;
                _izinTurleriRepository = izinTurleriRepository;
                _tatillerRepository = tatillerRepository;
            }

            public async Task<List<GetIzinByOrganizasyonIdResponse>> Handle(GetIzinByOrganizasyonIdQuery request, CancellationToken cancellationToken)
            {
                List<GetIzinByOrganizasyonIdResponse> response = new();

                if (request.OrganizasyonId == 0) return response;

                var organizasyon = await _organizasyonlarRepository.GetAsync(predicate: x => x.Id == request.OrganizasyonId, cancellationToken: cancellationToken);

                var organizasyonlar = await _organizasyonlarRepository.GetAllAsync(predicate: x => x.Kod.StartsWith(organizasyon.Kod), cancellationToken: cancellationToken);

                foreach (var org in organizasyonlar)
                {
                    if (org.BirimId == null) continue;


                    var calisanlar = await _calisanlarRepository.GetAllAsync(predicate: x => x.BirimId == org.BirimId.Value,
                                                                             include: source => source.Include(x => x.Izinler).ThenInclude(x => x.IzinTur)
                                                                                                      .Include(x => x.Birim)
                                                                                                      .Include(x => x.Unvan),
                                                                             cancellationToken: cancellationToken);

                    foreach (var calisan in calisanlar)
                    {
                        var responseTasks = calisan.Izinler.Where(x => x.IzinDurumId == (int)IzinDurumEnum.Onaylandi)
                                                          .Select(async x => new GetIzinByOrganizasyonIdResponse()
                                                          {
                                                              Unvan = calisan.Unvan.Aciklama,
                                                              Ad = calisan.Ad,
                                                              Soyad = calisan.Soyad,
                                                              Baslangic = x.BaslangicTarihi.ToString("dd.MM.yyyy"),
                                                              Bitis = x.BitisTarihi.ToString("dd.MM.yyyy"),
                                                              Birim = calisan.Birim.Aciklama,
                                                              IzinTur = x.IzinTur.Aciklama,
                                                              GunSayisi = await GetIzinGunSayisiAsync(x.IzinTurId, x.BaslangicTarihi, x.BitisTarihi, cancellationToken)
                                                          }).ToList();

                        var responseList = await Task.WhenAll(responseTasks);


                        response.AddRange(responseList);
                    }
                }



                return response;
            }

            private async Task<int> GetIzinGunSayisiAsync(int izinTurId, DateTime baslangicTarihi, DateTime bitisTarihi, CancellationToken cancellationToken)
            {
                IzinTur izinTur = await _izinTurleriRepository.GetAsync(predicate: x => x.Id == izinTurId, cancellationToken: cancellationToken);

                // Sabit izin gün sayısı varsa
                if (izinTur.SabitGunSayisi != null)
                    return izinTur.SabitGunSayisi.Value;

                // Başlangıç Bitiş arasındaki gün sayısını hesapla
                TimeSpan zamanFarkı = bitisTarihi - baslangicTarihi;
                int gunSayisi = zamanFarkı.Days + 1;

                // Tatil günü kullanılan babalık izni izin günü olarak sayılır
                if (izinTur.TatilGunleriSayilir)
                    return gunSayisi;

                for (DateTime date = baslangicTarihi; date <= bitisTarihi; date = date.AddDays(1))
                {
                    bool tatilGunu = await _tatillerRepository.AnyAsync(predicate: x => x.Tarih == date, cancellationToken: cancellationToken);

                    if (date.DayOfWeek == DayOfWeek.Sunday || tatilGunu)
                        gunSayisi--;
                }

                return gunSayisi;
            }
        }
    }
}
