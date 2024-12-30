using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetIslemYapilanIzinler
{
    public class GetIslemYapilanIzinlerQuery : IRequest<List<GetIslemYapilanIzinlerResponse>>
    {
        public int CalisanId { get; set; }

        public class GetIslemYapilanIzinlerQueryHandler : IRequestHandler<GetIslemYapilanIzinlerQuery, List<GetIslemYapilanIzinlerResponse>>
        {
            private readonly IMapper _mapper;
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetIslemYapilanIzinlerQueryHandler(IMapper mapper, ICalisanlarRepository calisanlarRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGruplariRepository izinGruplariRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _calisanlarRepository = calisanlarRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<List<GetIslemYapilanIzinlerResponse>> Handle(GetIslemYapilanIzinlerQuery request, CancellationToken cancellationToken)
            {
                Calisan calisan = await _calisanlarRepository.GetAsync(predicate: x => x.Id == request.CalisanId,
                                                                       include: source => source.Include(x => x.IzinOnayTanimCalisanlar),
                                                                       cancellationToken: cancellationToken);

                List<int> onayTanimIdleri = calisan.IzinOnayTanimCalisanlar
                                                       ?.Select(x => x.IzinOnayTanimId)
                                                       ?.ToList() ?? new List<int>();

                List<Izin> izinler = new();

                foreach (var onayTanimId in onayTanimIdleri)
                {
                    izinler.AddRange((await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinOnayTanimId == onayTanimId && x.IzinDurumId != (int)IzinDurumEnum.Beklemede,
                                                                                          include: source => source.Include(x => x.Izin).ThenInclude(x => x.Unvan)
                                                                                                                   .Include(x => x.Izin).ThenInclude(x => x.Calisan).ThenInclude(x => x.Birim)
                                                                                                                   .Include(x => x.Izin).ThenInclude(x => x.IzinDurum)
                                                                                                                   .Include(x => x.Izin).ThenInclude(x => x.Dogrulamalar),
                                                                                          cancellationToken: cancellationToken))
                                                                         .Select(x => x.Izin)
                                                                         .ToList());
                }

                izinler = izinler.DistinctBy(x => x.Id)
                                 .OrderByDescending(x => x.IstekTarihi)
                                 .ToList();

                List<GetIslemYapilanIzinlerResponse> response = new();

                // ... Onayı Bekleniyor Şeklinde Veri Göndermek İçin Yapılan İşlem
                foreach (var izin in izinler)
                {
                    var mappedIzin = _mapper.Map<GetIslemYapilanIzinlerResponse>(izin);


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

                        mappedIzin.GenelIzinDurumu = $"{izinOnayTanim.Aciklama} Onayı Bekleniyor";
                    }

                    // iznin son doğrulama yöntemini al
                    var sonDogrulamaYontemi = izin.Dogrulamalar.OrderByDescending(x => x.Id).First().Yontem;
                    mappedIzin.DogrulamaYontemi = sonDogrulamaYontemi;

                    response.Add(mappedIzin);
                }

                return response;
            }
        }
    }
}
