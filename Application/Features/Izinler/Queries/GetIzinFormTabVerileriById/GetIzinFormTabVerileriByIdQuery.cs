using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetIzinFormTabVerileriById
{
    public class GetIzinFormTabVerileriByIdQuery : IRequest<GetIzinFormTabVerileriByIdResponse>
    {
        public int Id { get; set; }

        public class GetIzinFormTabVerileriByIdQueryHandler : IRequestHandler<GetIzinFormTabVerileriByIdQuery, GetIzinFormTabVerileriByIdResponse>
        {
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly ITatillerRepository _tatillerRepository;
            private readonly IIzinTurleriRepository _izinTurleriRepository;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;
            private readonly IMapper _mapper;

            public GetIzinFormTabVerileriByIdQueryHandler(ICalisanlarRepository calisanlarRepository, ITatillerRepository tatillerRepository, IIzinTurleriRepository izinTurleriRepository, IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGruplariRepository izinGruplariRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository, IMapper mapper)
            {
                _calisanlarRepository = calisanlarRepository;
                _tatillerRepository = tatillerRepository;
                _izinTurleriRepository = izinTurleriRepository;
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
                _mapper = mapper;
            }

            public async Task<GetIzinFormTabVerileriByIdResponse> Handle(GetIzinFormTabVerileriByIdQuery request, CancellationToken cancellationToken)
            {
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                              include: source => source.Include(x => x.IzinTur)
                                                                                       .Include(x => x.IzinDurum)
                                                                                       .Include(x => x.Unvan)
                                                                                       .Include(x => x.Calisan)
                                                                                       .Include(x => x.Isyeri)
                                                                                       .Include(x => x.Birim),
                                                             cancellationToken: cancellationToken);

                GetIzinFormTabVerileriByIdResponse response = _mapper.Map<GetIzinFormTabVerileriByIdResponse>(izin);

                // ... Onayı Bekleniyor Şeklinde Veri Göndermek İçin Yapılan İşlem
                if (izin.IzinDurumId == (int)IzinDurumEnum.Beklemede)
                {
                    // iznin sonraki onay tanımını bul
                    IzinHareket sonIzinHareketi = (await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == izin.Id &&
                                                                                                                x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                                orderBy: source => source.OrderByDescending(x => x.IslemTarihi),
                                                                                                cancellationToken: cancellationToken))
                                                                                   .FirstOrDefault();

                    // Onay Tanım Sirasini bul
                    int onayTanimSirasi = sonIzinHareketi == null ? 1 : (int)(sonIzinHareketi!.Sira + 1);

                    // İznin izin grubu Id sini bul
                    int izinGrubuId = await _izinGruplariRepository.GetIzinGrupIdAsync(izin.CalisanId, izin.UnvanId, izin.BirimId, izin.IsyeriId, cancellationToken: cancellationToken);

                    // Onay Tanim Id bul
                    int onayTanimId = await _izinGrupIzinOnayTanimRepository.GetIzinOnayTanimIdAsync(izinGrubuId, onayTanimSirasi, cancellationToken: cancellationToken);

                    IzinOnayTanim izinOnayTanim = await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == onayTanimId, cancellationToken: cancellationToken);

                    response.IzinDurumu = $"{izinOnayTanim.Aciklama} Onayı Bekleniyor";
                }

                // İzin Gün Sayılarının Hesaplanması
                int izinTurId = izin.IzinTurId;

                response.ToplamGunSayisi = await GetIzinGunSayisiAsync(izinTurId, izin.BaslangicTarihi, izin.BitisTarihi, cancellationToken);

                DateTime? mahsubenBaslangicTarihi = izin.MahsubenBaslangicTarihi;

                if (mahsubenBaslangicTarihi != null)
                    response.MahsubenGunSayisi = await GetIzinGunSayisiAsync(izinTurId, izin.MahsubenBaslangicTarihi!.Value, izin.BitisTarihi, cancellationToken);

                // Görev Grubu
                string tc = izin.Calisan.Tc;
                response.GorevGrubu = await _calisanlarRepository.GetGorevGrubuAsync(tc);

                // Amirler
                int izinId = izin.Id;
                response.BirinciOnayVerenAdSoyad = await GetBirinciOnayVerenCalisanAdSoyadAsync(izinId, cancellationToken);
                response.IkinciOnayVerenAdSoyad = await GetIkinciOnayVerenCalisanAdSoyadAsync(izinId, cancellationToken);
                response.MerkezMuduruAdSoyad = await GetMerkezMuduruAdSoyadAsync(izinId, cancellationToken);

                return response;
            }

            private async Task<string> GetBirinciOnayVerenCalisanAdSoyadAsync(int izinId, CancellationToken cancellationToken)
            {
                IzinHareket izinHareket = await _izinHareketleriRepository.GetAsync(predicate: x => x.IzinId == izinId &&
                                                                                                 x.Sira == 2 && x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                      include: source => source.Include(x => x.Calisan),
                                                                                      cancellationToken: cancellationToken);
                if (izinHareket?.Calisan == null) return string.Empty;

                Calisan onayVerenCalisan = izinHareket.Calisan;

                return $"{onayVerenCalisan.Ad} {onayVerenCalisan.Soyad}";
            }

            private async Task<string> GetIkinciOnayVerenCalisanAdSoyadAsync(int izinId, CancellationToken cancellationToken)
            {
                IzinHareket onaylananSonIzinHareketi = (await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == izinId &&
                                                                                                                     x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                                     include: source => source.Include(x => x.Calisan),
                                                                                                     orderBy: source => source.OrderByDescending(x => x.Sira),
                                                                                                     cancellationToken: cancellationToken))
                                                       .FirstOrDefault();

                if (onaylananSonIzinHareketi?.Calisan == null) return string.Empty;

                Calisan onayVerenCalisan = onaylananSonIzinHareketi.Calisan;

                return $"{onayVerenCalisan.Ad} {onayVerenCalisan.Soyad}";
            }

            private async Task<string> GetMerkezMuduruAdSoyadAsync(int izinId, CancellationToken cancellationToken)
            {
                List<int> izinOnayTanimIdleri = (await _izinOnayTanimlariRepository.GetAllAsync(predicate: x => x.MerkezMuduruYetkisi, cancellationToken: cancellationToken))
                                                .Select(x => x.Id)
                                                .ToList();

                IzinHareket merkezMuduruIzinHareketi = (await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == izinId &&
                                                                                                                      izinOnayTanimIdleri.Contains(x.IzinOnayTanimId.Value) &&
                                                                                                                      x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                                      include: source => source.Include(x => x.Calisan),
                                                                                                      cancellationToken: cancellationToken))
                                                        .FirstOrDefault();

                if (merkezMuduruIzinHareketi?.Calisan == null) return string.Empty;

                Calisan onayVerenCalisan = merkezMuduruIzinHareketi.Calisan;

                return $"{onayVerenCalisan.Ad} {onayVerenCalisan.Soyad}";
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
