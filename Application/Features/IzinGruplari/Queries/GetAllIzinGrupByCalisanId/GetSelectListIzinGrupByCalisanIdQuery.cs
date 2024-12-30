using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IzinGruplari.Queries.GetAllIzinGrupByCalisanId
{
    public class GetSelectListIzinGrupByCalisanIdQuery : IRequest<List<GetSelectListIzinGrupByCalisanIdResponse>>
    {
        public int CalisanId { get; set; }

        public class GetSelectListIzinGrupByCalisanIdQueryHandler : IRequestHandler<GetSelectListIzinGrupByCalisanIdQuery, List<GetSelectListIzinGrupByCalisanIdResponse>>
        {
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IMapper _mapper;

            public GetSelectListIzinGrupByCalisanIdQueryHandler(IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IMapper mapper)
            {
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _mapper = mapper;
            }

            public async Task<List<GetSelectListIzinGrupByCalisanIdResponse>> Handle(GetSelectListIzinGrupByCalisanIdQuery request, CancellationToken cancellationToken)
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
                                              .DistinctBy(x => x.Id)
                                              .ToList();

                var response = _mapper.Map<List<GetSelectListIzinGrupByCalisanIdResponse>>(izinGruplari);

                return response;
            }
        }
    }
}
