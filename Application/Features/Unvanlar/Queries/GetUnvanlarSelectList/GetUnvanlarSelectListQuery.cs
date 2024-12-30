using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Unvanlar.Queries.GetUnvanlarSelectList
{
    public class GetUnvanlarSelectListQuery : IRequest<List<GetUnvanlarSelectListResponse>>
    {
        public string Search { get; set; }

        public class GetUnvanlarSelectListQueryHandler : IRequestHandler<GetUnvanlarSelectListQuery, List<GetUnvanlarSelectListResponse>>
        {
            private readonly IUnvanlarRepository _unvanlarRepository;

            public GetUnvanlarSelectListQueryHandler(IUnvanlarRepository unvanlarRepository)
            {
                _unvanlarRepository = unvanlarRepository;
            }

            public async Task<List<GetUnvanlarSelectListResponse>> Handle(GetUnvanlarSelectListQuery request, CancellationToken cancellationToken)
            {
                if (request?.Search == null)
                    return new List<GetUnvanlarSelectListResponse>();

                List<GetUnvanlarSelectListResponse> response = (await _unvanlarRepository.GetAllAsync(cancellationToken: cancellationToken))
                                                        .AsQueryable()
                                                        .Where(x => x.Aciklama.ToLower().Contains(request.Search.ToLower()))
                                                        .Select(x => new GetUnvanlarSelectListResponse()
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
