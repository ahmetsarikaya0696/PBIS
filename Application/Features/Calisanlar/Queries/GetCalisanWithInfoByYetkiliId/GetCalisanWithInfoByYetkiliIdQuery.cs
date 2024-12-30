using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Calisanlar.Queries.GetCalisanWithInfoByYetkiliId
{
    public class GetCalisanWithInfoByYetkiliIdQuery : IRequest<List<GetCalisanWithInfoByYetkiliIdResponse>>
    {
        public int CalisanId { get; set; }

        public int? IzinGrupId { get; set; }

        public class GetCalisanWithInfoByYetkiliIdQueryHandler : IRequestHandler<GetCalisanWithInfoByYetkiliIdQuery, List<GetCalisanWithInfoByYetkiliIdResponse>>
        {
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IIzinlerRepository _izinlerRepository;

            public GetCalisanWithInfoByYetkiliIdQueryHandler(IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, ICalisanlarRepository calisanlarRepository, IIzinlerRepository izinlerRepository)
            {
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _calisanlarRepository = calisanlarRepository;
                _izinlerRepository = izinlerRepository;
            }

            public async Task<List<GetCalisanWithInfoByYetkiliIdResponse>> Handle(GetCalisanWithInfoByYetkiliIdQuery request, CancellationToken cancellationToken)
            {
                // Çalışanın izin onay tanımlarını getir
                var izinOnayTanimIdleri = (await _izinOnayTanimCalisanRepository.GetAllAsync(predicate: x => x.CalisanId == request.CalisanId, cancellationToken: cancellationToken))
                                                        .Select(x => x.IzinOnayTanimId)
                                                        .Distinct()
                                                        .ToList();

                // İzin onay tanımlarının izin gruplarını getir
                var izinGruplari = (await _izinGrupIzinOnayTanimRepository.GetAllAsync(predicate: x => izinOnayTanimIdleri.Contains(x.IzinOnayTanimId),
                                                                                         include: source => source.Include(x => x.IzinGrup),
                                                                                         cancellationToken: cancellationToken))
                                                              .Select(x => x.IzinGrup)
                                                              .Where(x => request.IzinGrupId == null || x.Id == request.IzinGrupId)
                                                              .DistinctBy(x => x.Id)
                                                              .ToList();

                List<GetCalisanWithInfoByYetkiliIdResponse> responses = new();

                foreach (var izinGrubu in izinGruplari)
                {
                    var calisanId = izinGrubu.CalisanId;
                    var unvanId = izinGrubu.UnvanId;
                    var birimId = izinGrubu.BirimId;
                    var isyeriId = izinGrubu.IsyeriId;

                    var calisanlar = (await _calisanlarRepository.GetAllAsync(predicate: x => (calisanId == null || x.Id == calisanId) &&
                                                                                              (unvanId == null || x.UnvanId == unvanId) &&
                                                                                              (birimId == null || x.BirimId == birimId) &&
                                                                                              (isyeriId == null || x.IsyeriId == isyeriId)))
                                                                     .DistinctBy(x => x.Id)
                                                                     .ToList();

                    foreach (var calisan in calisanlar)
                    {
                        var responseItem = new GetCalisanWithInfoByYetkiliIdResponse()
                        {
                            Ad = calisan.Ad,
                            Soyad = calisan.Soyad,
                            KalanSenelikIzinGunSayisi = await _izinlerRepository.GetKalanIzinGunSayisiByTcAsync(calisan.Tc)
                        };

                        responses.Add(responseItem);
                    }
                }

                return responses;
            }
        }
    }
}
