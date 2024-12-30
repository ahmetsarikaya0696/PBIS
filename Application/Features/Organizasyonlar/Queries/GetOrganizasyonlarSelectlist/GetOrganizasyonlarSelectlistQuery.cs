using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.OrganizasyonSemasi.Queries.GetAnaBirimler
{
    public class GetOrganizasyonlarSelectlistQuery : IRequest<List<GetOrganizasyonlarSelectlistResponse>>
    {
        public string Search { get; set; }

        public class GetAnaBirimlerSelectlistQueryHandler : IRequestHandler<GetOrganizasyonlarSelectlistQuery, List<GetOrganizasyonlarSelectlistResponse>>
        {
            private readonly IOrganizasyonlarRepository _organizasyonSemasiRepository;

            public GetAnaBirimlerSelectlistQueryHandler(IOrganizasyonlarRepository organizasyonSemasiRepository)
            {
                _organizasyonSemasiRepository = organizasyonSemasiRepository;
            }

            public async Task<List<GetOrganizasyonlarSelectlistResponse>> Handle(GetOrganizasyonlarSelectlistQuery request, CancellationToken cancellationToken)
            {
                if (request?.Search == null)
                    return new List<GetOrganizasyonlarSelectlistResponse>();

                List<GetOrganizasyonlarSelectlistResponse> response = (await _organizasyonSemasiRepository.GetAllAsync(cancellationToken: cancellationToken))
                                                        .AsQueryable()
                                                        .Where(x => x.Aciklama_TR.ToLower().Contains(request.Search.ToLower()))
                                                        .Select(x => new GetOrganizasyonlarSelectlistResponse()
                                                        {
                                                            Id = x.Id.ToString(),
                                                            Text = x.Aciklama_TR
                                                        })
                                                        .Take(5)
                                                        .ToList();

                return response;
            }
        }
    }
}
