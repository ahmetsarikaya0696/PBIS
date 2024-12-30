using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Organizasyonlar.Queries.GetOrganizasyonById
{
    public class GetOrganizasyonByIdQuery : IRequest<GetOrganizasyonByIdResponse>
    {
        public int Id { get; set; }

        public class GetOrganizasyonByIdQueryHandler : IRequestHandler<GetOrganizasyonByIdQuery, GetOrganizasyonByIdResponse>
        {
            private readonly IOrganizasyonlarRepository _organizasyonlarRepository;
            private readonly IMapper _mapper;

            public GetOrganizasyonByIdQueryHandler(IOrganizasyonlarRepository organizasyonlarRepository, IMapper mapper)
            {
                _organizasyonlarRepository = organizasyonlarRepository;
                _mapper = mapper;
            }

            public async Task<GetOrganizasyonByIdResponse> Handle(GetOrganizasyonByIdQuery request, CancellationToken cancellationToken)
            {
                var organizasyon = await _organizasyonlarRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);

                var response = _mapper.Map<GetOrganizasyonByIdResponse>(organizasyon);

                response.UstBirim = (await _organizasyonlarRepository.GetAsync(predicate: x => x.Id == response.UstBirimId))?.Aciklama_TR;

                return response;
            }
        }
    }
}
