using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Queries.GetIzinDetayById
{
    public class GetIzinDetayByIdQuery : IRequest<GetIzinDetayByIdResponse>
    {
        public int Id { get; set; }

        public class GetIzinDetayByIdQueryHandler : IRequestHandler<GetIzinDetayByIdQuery, GetIzinDetayByIdResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IBaskentResimBlobRepository _baskentResimBlobRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetIzinDetayByIdQueryHandler(IMapper mapper, IIzinlerRepository izinlerRepository, IBaskentResimBlobRepository baskentResimBlobRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGruplariRepository izinGruplariRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinlerRepository = izinlerRepository;
                _baskentResimBlobRepository = baskentResimBlobRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<GetIzinDetayByIdResponse> Handle(GetIzinDetayByIdQuery request, CancellationToken cancellationToken)
            {
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                              include: source => source.Include(x => x.IzinTur)
                                                                                       .Include(x => x.Unvan)
                                                                                       .Include(x => x.Calisan)
                                                                                       .Include(x => x.Birim),
                                                              cancellationToken: cancellationToken);

                GetIzinDetayByIdResponse response = _mapper.Map<GetIzinDetayByIdResponse>(izin);
                string tc = izin.Calisan.Tc;
                response.KalanSenelikIzinGunSayisi = await _izinlerRepository.GetKalanIzinGunSayisiByTcAsync(tc);

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

                    // Mahsuben Tarihleri Düzenleme Yetkisi
                    response.PersonelSubeYetkisi = izinOnayTanim.PersonelSubeYetkisi;
                }

                // Kişinin fotoğrafını getir
                var LREF = izin.Calisan.LREF;
                response.Fotograf = Convert.ToBase64String((await _baskentResimBlobRepository.GetAsync(predicate: x => x.LREF == LREF, cancellationToken: cancellationToken)).RESIM);

                // İzin isteği ile aynı zamana denk gelen izinler
                List<Izin> izinTarihleriKesisenIzinler = await GetKesisenIzinlerAsync(izin, cancellationToken);

                response.KesisenIzinler = _mapper.Map<List<KesisenIzin>>(izinTarihleriKesisenIzinler);
                response.GunSayisi = await _izinlerRepository.GetIzinGunSayisiAsync(izin.BaslangicTarihi, izin.BitisTarihi, izin.IzinTurId, cancellationToken: cancellationToken);
                response.MahsubenGunSayisi = izin.MahsubenBaslangicTarihi.HasValue ? await _izinlerRepository.GetIzinGunSayisiAsync(izin.MahsubenBaslangicTarihi.Value, izin.BitisTarihi, izin.IzinTurId, cancellationToken: cancellationToken) : null;

                return response;
            }

            private async Task<List<Izin>> GetKesisenIzinlerAsync(Izin izin, CancellationToken cancellationToken)
            {
                List<Izin> izinTarihleriKesisenIzinler = new();

                List<Izin> ayniBirimdekiIzinler = await _izinlerRepository.GetAllAsync(predicate: x => x.IsyeriId == izin.IsyeriId && x.BirimId == izin.BirimId && x.Id != izin.Id,
                                                                                       include: source => source.Include(x => x.Unvan)
                                                                                                                .Include(x => x.Calisan)
                                                                                                                .Include(x => x.IzinDurum)
                                                                                                                .Include(x => x.IzinTur),
                                                                                       cancellationToken: cancellationToken);

                foreach (var ayniBirimdekiIzin in ayniBirimdekiIzinler)
                {
                    bool izinlerKesisiyor = (izin.BaslangicTarihi <= ayniBirimdekiIzin.BaslangicTarihi && ayniBirimdekiIzin.BaslangicTarihi <= izin.IseBaslamaTarihi) ||
                                            (izin.BaslangicTarihi <= ayniBirimdekiIzin.IseBaslamaTarihi && ayniBirimdekiIzin.IseBaslamaTarihi <= izin.IseBaslamaTarihi);

                    if (izinlerKesisiyor)
                        izinTarihleriKesisenIzinler.Add(ayniBirimdekiIzin);
                }

                return izinTarihleriKesisenIzinler;
            }
        }

    }
}
