using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetOnaylanacakIzinler
{
    public class GetOnaylanacakIzinlerQuery : IRequest<List<GetOnaylanacakIzinlerResponse>>
    {
        public int CalisanId { get; set; }

        public class GetOnaylanacakIzinlerQueryHandler : IRequestHandler<GetOnaylanacakIzinlerQuery, List<GetOnaylanacakIzinlerResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetOnaylanacakIzinlerQueryHandler(IMapper mapper, IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinGruplariRepository izinGruplariRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<List<GetOnaylanacakIzinlerResponse>> Handle(GetOnaylanacakIzinlerQuery request, CancellationToken cancellationToken)
            {
                // Return edilecek izinler
                List<Izin> izinler = new();

                // Onay veren çalışanın onay tanim id ' leri
                List<int> onayVerenCalisaninOnayTanimIdleri = (await _izinOnayTanimCalisanRepository.GetAllAsync(predicate: x => x.CalisanId == request.CalisanId,
                                                                                                                 cancellationToken: cancellationToken))
                                                                                                    .Select(x => x.IzinOnayTanimId)
                                                                                                    .Distinct()
                                                                                                    .ToList();

                // izin grupları onay yetkisinde olan izinleri bul
                var onayBekleyenIzinler = await _izinlerRepository.GetAllAsync(predicate: x => x.IzinDurumId == (int)IzinDurumEnum.Beklemede,
                                                                               include: source => source.Include(x => x.Unvan)
                                                                                                        .Include(x => x.Calisan)
                                                                                                        .Include(x => x.Birim)
                                                                                                        .Include(x => x.IzinDurum)
                                                                                                        .Include(x => x.Dogrulamalar),
                                                                               orderBy: source => source.OrderByDescending(x => x.IstekTarihi),
                                                                               cancellationToken: cancellationToken);

                foreach (var onayBekleyenIzin in onayBekleyenIzinler)
                {
                    IzinHareket sonIzinHareketi = (await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == onayBekleyenIzin.Id &&
                                                                                                                x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                                orderBy: source => source.OrderByDescending(x => x.IslemTarihi),
                                                                                                cancellationToken: cancellationToken))
                                                                                   .FirstOrDefault();

                    // Onay Tanım Sirasini bul
                    int onayTanimSirasi = sonIzinHareketi == null ? 1 : (int)(sonIzinHareketi!.Sira + 1);

                    // Onay bekleyen iznin izin grubu Id sini bul
                    int izinGrubuId = (await _izinGruplariRepository.GetAsync(predicate: x => x.CalisanId == onayBekleyenIzin.CalisanId,
                                                                              cancellationToken: cancellationToken) ??
                                       await _izinGruplariRepository.GetAsync(predicate: x => x.BirimId == onayBekleyenIzin.BirimId,
                                                                              cancellationToken: cancellationToken) ??
                                       await _izinGruplariRepository.GetAsync(predicate: x => x.IsyeriId == onayBekleyenIzin.IsyeriId,
                                                                              cancellationToken: cancellationToken)).Id;

                    // Onay Tanim Id bul
                    int onayTanimId = await _izinGrupIzinOnayTanimRepository.GetIzinOnayTanimIdAsync(izinGrubuId, onayTanimSirasi, cancellationToken: cancellationToken);

                    // Son onay tanım Id ' si onav veren çalışanın onay tanım Id lerinde varsa gösterilecek izinlere ekle
                    if (onayVerenCalisaninOnayTanimIdleri.Contains(onayTanimId))
                    {
                        izinler.Add(onayBekleyenIzin);
                    }
                }

                List<GetOnaylanacakIzinlerResponse> response = new();

                // ... Onayı Bekleniyor Şeklinde Veri Göndermek İçin Yapılan İşlem
                foreach (var izin in izinler)
                {
                    GetOnaylanacakIzinlerResponse mappedResponse = _mapper.Map<GetOnaylanacakIzinlerResponse>(izin);


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

                        mappedResponse.IzinDurumu = $"{izinOnayTanim.Aciklama} Onayı Bekleniyor";
                    }

                    // iznin son doğrulama yöntemini al
                    var sonDogrulamaYontemi = izin.Dogrulamalar.OrderByDescending(x => x.Id).First().Yontem;
                    mappedResponse.DogrulamaYontemi = sonDogrulamaYontemi;

                    response.Add(mappedResponse);
                }

                return response;
            }
        }
    }
}
