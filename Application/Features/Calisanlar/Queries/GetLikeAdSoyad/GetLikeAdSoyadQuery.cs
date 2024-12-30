using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Calisanlar.Queries.GetLikeAdSoyad
{
    public class GetLikeAdSoyadQuery : IRequest<List<GetLikeAdSoyadResponse>>
    {
        public string Search { get; set; }

        public class GetLikeAdSoyadQueryHandler : IRequestHandler<GetLikeAdSoyadQuery, List<GetLikeAdSoyadResponse>>
        {
            private readonly ICalisanlarRepository _calisanlarRepository;

            public GetLikeAdSoyadQueryHandler(ICalisanlarRepository calisanlarRepository)
            {
                _calisanlarRepository = calisanlarRepository;
            }

            public async Task<List<GetLikeAdSoyadResponse>> Handle(GetLikeAdSoyadQuery request, CancellationToken cancellationToken)
            {
                if (request?.Search == null)
                    return new List<GetLikeAdSoyadResponse>();

                List<GetLikeAdSoyadResponse> response = (await _calisanlarRepository.GetAllAsync(cancellationToken: cancellationToken))
                                                        .AsQueryable()
                                                        .Where(x => $"{x.Ad} {x.Soyad}".ToLower().Contains(request.Search.ToLower()))
                                                        .Select(x => new GetLikeAdSoyadResponse()
                                                        {
                                                            Id = x.Id.ToString(),
                                                            Text = $"{x.Ad} {x.Soyad} ({x.SicilNo})"
                                                        })
                                                        .Take(5)
                                                        .ToList();

                return response;
            }
        }
    }
}
