using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetByCalisanId
{
    public class GetByCalisanIdIzinQuery : IRequest<List<GetByCalisanIdIzinResponse>>
    {
        public int CalisanId { get; set; }

        public class GetByCalisanIdIzinQueryHandler : IRequestHandler<GetByCalisanIdIzinQuery, List<GetByCalisanIdIzinResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetByCalisanIdIzinQueryHandler(IMapper mapper, IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGruplariRepository izinGruplariRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<List<GetByCalisanIdIzinResponse>> Handle(GetByCalisanIdIzinQuery request, CancellationToken cancellationToken)
            {
                List<Izin> izinler = await _izinlerRepository
                                                    .GetAllAsync(predicate: x => x.CalisanId == request.CalisanId,
                                                                 include: source => source.Include(x => x.IzinDurum)
                                                                                          .Include(x => x.IzinTur)
                                                                                          .Include(x => x.Dogrulamalar)
                                                                                          .Include(x => x.IzinHareketleri).ThenInclude(x => x.RetDetay).ThenInclude(x => x.RetSebep),
                                                                 orderBy: source => source.OrderByDescending(x => x.IstekTarihi),
                                                                 cancellationToken: cancellationToken);

                List<GetByCalisanIdIzinResponse> response = new();

                foreach (var izin in izinler)
                {
                    var mappedIzin = _mapper.Map<GetByCalisanIdIzinResponse>(izin);

                    // 1/4 gibi onaylanan adım sayısı / toplam adım sayısını göstermek için yapılan işlem
                    // iznin sonraki onay tanımını bul
                    IzinHareket sonOnaylanmisIzinHareketi = (await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == izin.Id && x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                                                                          orderBy: source => source.OrderByDescending(x => x.IslemTarihi),
                                                                                                          cancellationToken: cancellationToken)).FirstOrDefault();

                    int sonOnaySirasi = sonOnaylanmisIzinHareketi != null ? sonOnaylanmisIzinHareketi.Sira!.Value : 0;

                    // İznin izin grubu Id sini bul
                    int izinGrubuId = await _izinGruplariRepository.GetIzinGrupIdAsync(izin.CalisanId, izin.UnvanId, izin.BirimId, izin.IsyeriId, cancellationToken: cancellationToken);

                    var izinGrupIzinOnayTanimlari = await _izinGrupIzinOnayTanimRepository.GetAllAsync(predicate: x => x.IzinGrupId == izinGrubuId,
                                                                                                       cancellationToken: cancellationToken)
                                                    ?? throw new ClientsideException("Belirtilen izin grubuna ait onay tanım verisi bulunamadı");

                    int toplamAdimSayisi = izinGrupIzinOnayTanimlari!.Count;

                    mappedIzin.Adim = $"{sonOnaySirasi}/{toplamAdimSayisi}";

                    // ... Onayı Bekleniyor Şeklinde Veri Göndermek İçin Yapılan İşlem
                    if (izin.IzinDurumId == (int)IzinDurumEnum.Beklemede)
                    {
                        // Onay Tanım Sirasini bul
                        int onayTanimSirasi = sonOnaylanmisIzinHareketi == null ? 1 : sonOnaylanmisIzinHareketi.Sira!.Value + 1;

                        // Onay Tanim Id bul
                        int onayTanimId = await _izinGrupIzinOnayTanimRepository.GetIzinOnayTanimIdAsync(izinGrubuId, onayTanimSirasi, cancellationToken: cancellationToken);

                        IzinOnayTanim izinOnayTanim = await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == onayTanimId, cancellationToken: cancellationToken);

                        mappedIzin.IzinDurumu = $"{izinOnayTanim.Aciklama} Onayı Bekleniyor";
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
