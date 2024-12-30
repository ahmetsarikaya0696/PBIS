using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonlar
{
    public class GetOrganizasyonlarQuery : IRequest<List<GetOrganizasyonlarResponse>>
    {
        public class GetOrganizasyonlarQueryHandler : IRequestHandler<GetOrganizasyonlarQuery, List<GetOrganizasyonlarResponse>>
        {
            private readonly IOrganizasyonlarRepository _organizasyonSemasiRepository;
            private readonly IMapper _mapper;

            public GetOrganizasyonlarQueryHandler(IOrganizasyonlarRepository organizasyonSemasiRepository, IMapper mapper)
            {
                _organizasyonSemasiRepository = organizasyonSemasiRepository;
                _mapper = mapper;
            }

            public async Task<List<GetOrganizasyonlarResponse>> Handle(GetOrganizasyonlarQuery request, CancellationToken cancellationToken)
            {
                List<Organizasyon> organizasyonlar = await _organizasyonSemasiRepository.GetAllAsync(orderBy: source => source.OrderBy(x => x.Kod), cancellationToken: cancellationToken);

                var responseList = _mapper.Map<List<GetOrganizasyonlarResponse>>(organizasyonlar);

                foreach (var response in responseList)
                {
                    var ustOrganizasyon = await _organizasyonSemasiRepository.GetAsync(predicate: x => x.Id == response.UstBirimId, cancellationToken: cancellationToken);

                    if (ustOrganizasyon == null) continue;

                    response.UstBirimAciklama = ustOrganizasyon.Aciklama_TR;
                }


                return responseList;
            }
        }
    }
}
