using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Commands.Reddet
{
    public class ReddetIzinCommand : IRequest<ReddetIzinResponse>
    {
        public int IzinId { get; set; }

        public string Ip { get; set; }

        public int CalisanId { get; set; }

        public int RetSebepId { get; set; }

        public string Detay { get; set; }

        public class ReddetIzinCommandHandler : IRequestHandler<ReddetIzinCommand, ReddetIzinResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IRetDetaylariRepository _retDetaylariRepository;
            private readonly IRetSebepleriRepository _retSebepleriRepository;
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;

            public ReddetIzinCommandHandler(IMapper mapper, IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGruplariRepository izinGruplariRepository, IRetDetaylariRepository retDetaylariRepository, IRetSebepleriRepository retSebepleriRepository, IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository)
            {
                _mapper = mapper;
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _retDetaylariRepository = retDetaylariRepository;
                _retSebepleriRepository = retSebepleriRepository;
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
            }

            public async Task<ReddetIzinResponse> Handle(ReddetIzinCommand request, CancellationToken cancellationToken)
            {
                // Ret Sebebine göre durum güncellemesi yapılacak
                RetSebep retSebep = await _retSebepleriRepository.GetAsync(predicate: x => x.Id == request.RetSebepId, cancellationToken: cancellationToken);
                int izinDurumId = retSebep.Duzenlenebilir ? (int)IzinDurumEnum.Duzeltilmek_Uzere_Geri_Gonderildi : (int)IzinDurumEnum.Reddedildi;

                IzinHareket eklenmisIzinHareketi = null;

                // İzin isteğini yapan kişiyi bulmak için izni al
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.IzinId, cancellationToken: cancellationToken);

                // İzin isteğini yapan kişinin izin grup id ' sini bul
                int izinGrupId = await _izinGruplariRepository.GetIzinGrupIdAsync(izin.CalisanId, izin.UnvanId, izin.BirimId, izin.IsyeriId, cancellationToken: cancellationToken);

                // İzni reddedecek kişinin izin onay tanım Id si ile birlikte izin onay sırasını al
                var reddedecekOlanCalisaninOnayTanimIdleriVeOnayTanimSiralari = (await _izinOnayTanimCalisanRepository
                                                            .GetAllAsync(predicate: x => x.CalisanId == request.CalisanId,
                                                                         include: source => source.Include(x => x.IzinOnayTanim).ThenInclude(x => x.IzinGrupIzinOnayTanimlari),
                                                                         cancellationToken: cancellationToken))
                                                            .SelectMany(x => x.IzinOnayTanim.IzinGrupIzinOnayTanimlari)
                                                            .Where(x => x.IzinGrupId == izinGrupId)
                                                            .Select(x => new { x.IzinOnayTanimId, x.OnayTanimSirasi })
                                                            .OrderBy(x => x.OnayTanimSirasi)
                                                            .ToList();

                // Eğer izin onay tanımı id si aynı olan biri tarafından onaylanmışsa
                foreach (var reddedecekOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi in reddedecekOlanCalisaninOnayTanimIdleriVeOnayTanimSiralari)
                {
                    int izinOnayTanimId = reddedecekOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.IzinOnayTanimId;
                    int sira = reddedecekOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.OnayTanimSirasi;

                    bool onaylanmisKayitVar = await _izinHareketleriRepository
                                                   .AnyAsync(predicate: x => x.IzinId == izin.Id &&
                                                                             x.IzinOnayTanimId == izinOnayTanimId &&
                                                                             x.Sira == sira &&
                                                                             x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                             cancellationToken: cancellationToken);

                    if (!onaylanmisKayitVar)
                    {
                        eklenmisIzinHareketi = await _izinHareketleriRepository.AddAsync(
                            new()
                            {
                                Ip = request.Ip,
                                CalisanId = request.CalisanId,
                                IzinId = izin.Id,
                                Sira = sira,
                                IslemTarihi = DateTime.Now,
                                IzinOnayTanimId = izinOnayTanimId,
                                IzinDurumId = izinDurumId,
                            });

                        izin.IzinDurumId = izinDurumId;
                        await _izinlerRepository.UpdateAsync(izin);

                        await _retDetaylariRepository.AddAsync(new()
                        {
                            Detay = request.Detay,
                            IzinHareketId = eklenmisIzinHareketi.Id,
                            RetSebepId = request.RetSebepId,
                        });

                        break;
                    }

                }

                var izinHareketWithIncludes = await _izinHareketleriRepository.GetAsync(predicate: x => x.Id == eklenmisIzinHareketi.Id,
                                                                             include: source => source.Include(x => x.Izin).ThenInclude(x => x.Calisan)
                                                                                                      .Include(x => x.Izin).ThenInclude(x => x.Birim)
                                                                                                      .Include(x => x.Izin).ThenInclude(x => x.Isyeri),
                                                                             cancellationToken: cancellationToken);

                ReddetIzinResponse response = _mapper.Map<ReddetIzinResponse>(izinHareketWithIncludes);

                return response;
            }
        }
    }
}
