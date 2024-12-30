using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonSemasi
{
    public class GetOrganizasyonSemasiQuery : IRequest<List<GetOrganizasyonSemasiByBirimIdResponse>>
    {
        public int BirimId { get; set; }


        public class GetOrganizasyonSemasiQueryHandler : IRequestHandler<GetOrganizasyonSemasiQuery, List<GetOrganizasyonSemasiByBirimIdResponse>>
        {
            private readonly IOrganizasyonlarRepository _organizasyonSemasiRepository;

            public GetOrganizasyonSemasiQueryHandler(IOrganizasyonlarRepository organizasyonSemasiRepository)
            {
                _organizasyonSemasiRepository = organizasyonSemasiRepository;
            }

            public async Task<List<GetOrganizasyonSemasiByBirimIdResponse>> Handle(GetOrganizasyonSemasiQuery request, CancellationToken cancellationToken)
            {
                if (request.BirimId == 0) return null;

                var kokBirimId = request.BirimId;

                var altBirimler = await _organizasyonSemasiRepository.AltBirimleriBulAsync(kokBirimId, cancellationToken);

                var kokBirim = await _organizasyonSemasiRepository.GetAsync(predicate: x => x.Id == kokBirimId);
                kokBirim.UstBirimId = 0;
                altBirimler.Add(kokBirim);

                return altBirimler.Select(x => new GetOrganizasyonSemasiByBirimIdResponse
                {
                    Key = x.Id.ToString(),
                    Name = x.Aciklama_TR,
                    Parent = x.UstBirimId,
                }).ToList();
            }
        }
    }
}
