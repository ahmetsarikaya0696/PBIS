using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Izinler.Queries.GetSuankiIzinByCalisanId
{
    public class GetSuankiIzinByCalisanIdQuery : IRequest<GetSuankiIzinByCalisanIdResponse>
    {
        public int CalisanId { get; set; }

        public class GetSuankiIzinByCalisanIdQueryHandler : IRequestHandler<GetSuankiIzinByCalisanIdQuery, GetSuankiIzinByCalisanIdResponse>
        {
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IMapper _mapper;

            public GetSuankiIzinByCalisanIdQueryHandler(IIzinlerRepository izinlerRepository, IMapper mapper)
            {
                _izinlerRepository = izinlerRepository;
                _mapper = mapper;
            }

            public async Task<GetSuankiIzinByCalisanIdResponse> Handle(GetSuankiIzinByCalisanIdQuery request, CancellationToken cancellationToken)
            {
                Izin suankiIzin = await _izinlerRepository.GetAsync(predicate: x => x.CalisanId == request.CalisanId &&
                                                                    x.BaslangicTarihi <= DateTime.Now &&
                                                                    x.BitisTarihi >= DateTime.Now &&
                                                                    x.IzinDurumId == (int)IzinDurumEnum.Onaylandi,
                                                    cancellationToken: cancellationToken);

                if (suankiIzin == null) return null;

                var response = _mapper.Map<GetSuankiIzinByCalisanIdResponse>(suankiIzin);

                response.GunSayisi = await _izinlerRepository.GetIzinGunSayisiAsync(suankiIzin.BaslangicTarihi, suankiIzin.BitisTarihi, suankiIzin.IzinTurId, cancellationToken: cancellationToken);

                return response;
            }
        }
    }
}
