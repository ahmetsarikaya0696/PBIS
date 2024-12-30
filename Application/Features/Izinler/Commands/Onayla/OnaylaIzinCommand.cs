using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Commands.Onayla
{
    public class OnaylaIzinCommand : IRequest<OnaylaIzinResponse>
    {
        public int IzinId { get; set; }

        public string Ip { get; set; }

        public int CalisanId { get; set; }

        public class OnaylaIzinCommandHandler : IRequestHandler<OnaylaIzinCommand, OnaylaIzinResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;

            public OnaylaIzinCommandHandler(IMapper mapper, IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, IIzinGruplariRepository izinGruplariRepository, IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository)
            {
                _mapper = mapper;
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
                _izinGruplariRepository = izinGruplariRepository;
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
            }

            public async Task<OnaylaIzinResponse> Handle(OnaylaIzinCommand request, CancellationToken cancellationToken)
            {
                IzinHareket eklenmisIzinHareketi = null;

                // İzin isteğini yapan kişiyi bulmak için izni al
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.IzinId, cancellationToken: cancellationToken);

                // İzin isteğini yapan kişinin izin grup id ' sini bul
                int izinGrupId = await _izinGruplariRepository.GetIzinGrupIdAsync(izin.CalisanId, izin.UnvanId, izin.BirimId, izin.IsyeriId, cancellationToken: cancellationToken);

                // Son izin onay tanım Id ' yi bul
                int sonIzinOnayTanimId = (await _izinGrupIzinOnayTanimRepository.GetAllAsync(predicate: x => x.IzinGrupId == izinGrupId,
                                                                                             orderBy: source => source.OrderByDescending(x => x.OnayTanimSirasi),
                                                                                             cancellationToken: cancellationToken))
                                                                                .Select(x => x.IzinOnayTanimId)
                                                                                .FirstOrDefault();

                // İzni onaylayacak kişinin izin onay tanım Id si ile birlikte izin onay sırasını al

                var onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSiralari = (await _izinOnayTanimCalisanRepository
                                                            .GetAllAsync(predicate: x => x.CalisanId == request.CalisanId,
                                                                         include: source => source.Include(x => x.IzinOnayTanim).ThenInclude(x => x.IzinGrupIzinOnayTanimlari),
                                                                         cancellationToken: cancellationToken))
                                                            .SelectMany(x => x.IzinOnayTanim.IzinGrupIzinOnayTanimlari)
                                                            .Where(x => x.IzinGrupId == izinGrupId)
                                                            .Select(x => new { x.IzinOnayTanimId, x.OnayTanimSirasi })
                                                            .OrderBy(x => x.OnayTanimSirasi)
                                                            .ToList();

                // Eğer izin hareketlerinde izin OnayTanimId, Sira ve Onaylandi Statüsünde bir kayıt yoksa izin hareketlerine onaylandi olarak kaydet
                foreach (var onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi in onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSiralari)
                {
                    bool onaylanmisKayitVar = await _izinHareketleriRepository
                                                   .AnyAsync(predicate: x => x.IzinId == izin.Id &&
                                                                             x.IzinOnayTanimId == onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.IzinOnayTanimId &&
                                                                             x.Sira == onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.OnayTanimSirasi &&
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
                                Sira = onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.OnayTanimSirasi,
                                IslemTarihi = DateTime.Now,
                                IzinOnayTanimId = onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.IzinOnayTanimId,
                                IzinDurumId = (int)IzinDurumEnum.Onaylandi,
                            });

                        // Son izin hareketiyse iznin durumunu da onaylanmışa çek
                        if (onaylayacakOlanCalisaninOnayTanimIdleriVeOnayTanimSirasi.IzinOnayTanimId == sonIzinOnayTanimId)
                        {
                            izin.IzinDurumId = (int)IzinDurumEnum.Onaylandi;
                            await _izinlerRepository.UpdateAsync(izin);
                        }

                        break;
                    }

                }

                OnaylaIzinResponse response = _mapper.Map<OnaylaIzinResponse>(eklenmisIzinHareketi);

                return response;
            }
        }
    }
}
