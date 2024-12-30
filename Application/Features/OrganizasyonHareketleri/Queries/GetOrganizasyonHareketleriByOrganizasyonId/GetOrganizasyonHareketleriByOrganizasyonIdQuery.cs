using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.OrganizasyonHareketleri.Queries.GetOrganizasyonHareketleriByOrganizasyonId
{
    public class GetOrganizasyonHareketleriByOrganizasyonIdQuery : IRequest<List<GetOrganizasyonHareketleriByOrganizasyonIdResponse>>
    {
        public int OrganizasyonId { get; set; }

        public class GetOrganizasyonHareketleriByOrganizasyonIdQueryHandler : IRequestHandler<GetOrganizasyonHareketleriByOrganizasyonIdQuery, List<GetOrganizasyonHareketleriByOrganizasyonIdResponse>>
        {
            private readonly IOrganizasyonHareketleriRepository _organizasyonHareketleriRepository;
            private readonly IMapper _mapper;

            public GetOrganizasyonHareketleriByOrganizasyonIdQueryHandler(IOrganizasyonHareketleriRepository organizasyonHareketleriRepository, IMapper mapper)
            {
                _organizasyonHareketleriRepository = organizasyonHareketleriRepository;
                _mapper = mapper;
            }

            public async Task<List<GetOrganizasyonHareketleriByOrganizasyonIdResponse>> Handle(GetOrganizasyonHareketleriByOrganizasyonIdQuery request, CancellationToken cancellationToken)
            {
                var organizasyonHareketleri = await _organizasyonHareketleriRepository.GetAllAsync(predicate: x => x.OrganizasyonId == request.OrganizasyonId,
                                                                                                   include: source => source.Include(x => x.Calisan).ThenInclude(x => x.Unvan),
                                                                                                   orderBy: source => source.OrderBy(x => x.IslemTarihi),
                                                                                                   cancellationToken: cancellationToken);

                var response = _mapper.Map<List<GetOrganizasyonHareketleriByOrganizasyonIdResponse>>(organizasyonHareketleri);

                return response;
            }
        }
    }
}
