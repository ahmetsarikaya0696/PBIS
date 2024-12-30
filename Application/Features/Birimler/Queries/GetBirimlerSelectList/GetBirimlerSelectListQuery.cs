using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Birimler.Queries.GetBirimlerSelectList
{
    public class GetBirimlerSelectListQuery : IRequest<List<GetBirimlerSelectListResponse>>
    {
        public string Search { get; set; }

        public class GetBirimlerSelectListQueryHandler : IRequestHandler<GetBirimlerSelectListQuery, List<GetBirimlerSelectListResponse>>
        {
            private readonly IBirimlerRepository _birimlerRepository;

            public GetBirimlerSelectListQueryHandler(IBirimlerRepository birimlerRepository)
            {
                _birimlerRepository = birimlerRepository;
            }

            public async Task<List<GetBirimlerSelectListResponse>> Handle(GetBirimlerSelectListQuery request, CancellationToken cancellationToken)
            {
                if (request?.Search == null)
                    return new List<GetBirimlerSelectListResponse>();

                List<GetBirimlerSelectListResponse> response = (await _birimlerRepository.GetAllAsync(cancellationToken: cancellationToken))
                                                        .AsQueryable()
                                                        .Where(x => x.Aciklama.ToLower().Contains(request.Search.ToLower()))
                                                        .Select(x => new GetBirimlerSelectListResponse()
                                                        {
                                                            Id = x.Id.ToString(),
                                                            Text = x.Aciklama
                                                        })
                                                        .Take(5)
                                                        .ToList();

                return response;
            }
        }
    }
}
