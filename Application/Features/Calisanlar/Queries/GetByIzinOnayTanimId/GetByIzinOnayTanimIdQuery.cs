using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Calisanlar.Queries.GetByIzinOnayTanimId
{
    public class GetByIzinOnayTanimIdQuery : IRequest<List<GetByIzinOnayTanimIdResponse>>
    {
        public int IzinOnayTanimId { get; set; }

        public class GetByIzinOnayTanimIdQueryHandler : IRequestHandler<GetByIzinOnayTanimIdQuery, List<GetByIzinOnayTanimIdResponse>>
        {
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;

            public GetByIzinOnayTanimIdQueryHandler(IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository)
            {
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
            }

            public async Task<List<GetByIzinOnayTanimIdResponse>> Handle(GetByIzinOnayTanimIdQuery request, CancellationToken cancellationToken)
            {
                List<GetByIzinOnayTanimIdResponse> response = (await _izinOnayTanimCalisanRepository.GetAllAsync(predicate: x => x.IzinOnayTanimId == request.IzinOnayTanimId,
                                                                                                                 include: source => source.Include(x => x.Calisan),
                                                                                                                 cancellationToken: cancellationToken))
                                                                                                    .Select(x => new GetByIzinOnayTanimIdResponse()
                                                                                                    {
                                                                                                        Id = x.CalisanId.ToString(),
                                                                                                        AdSoyad = $"{x.Calisan.Ad} {x.Calisan.Soyad} ({x.Calisan.SicilNo})"
                                                                                                    })
                                                                                                    .ToList();

                return response;
            }
        }
    }
}
