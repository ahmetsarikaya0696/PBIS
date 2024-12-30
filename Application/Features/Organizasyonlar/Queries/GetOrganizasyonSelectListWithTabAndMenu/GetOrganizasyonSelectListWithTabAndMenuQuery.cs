using Application.Features.Organizasyonlar.Composite;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organizasyonlar.Queries.GetOrganizasyonSelectListWithTabAndMenu
{
    public class GetOrganizasyonSelectListWithTabAndMenuQuery : IRequest<GetOrganizasyonSelectListWithTabAndMenuResponse>
    {
        public string Kod { get; set; } = "10";

        public class GetOrganizasyonSelectListWithTabAndMenuQueryHandler : IRequestHandler<GetOrganizasyonSelectListWithTabAndMenuQuery, GetOrganizasyonSelectListWithTabAndMenuResponse>
        {
            private readonly IOrganizasyonlarRepository _organizasyonlarRepository;

            public GetOrganizasyonSelectListWithTabAndMenuQueryHandler(IOrganizasyonlarRepository organizasyonlarRepository)
            {
                _organizasyonlarRepository = organizasyonlarRepository;
            }

            public async Task<GetOrganizasyonSelectListWithTabAndMenuResponse> Handle(GetOrganizasyonSelectListWithTabAndMenuQuery request, CancellationToken cancellationToken)
            {
                var organizasyonlar = await _organizasyonlarRepository.GetAllAsync(predicate: x => x.Kod.StartsWith(request.Kod), cancellationToken: cancellationToken);
                var topOrganizasyon = await _organizasyonlarRepository.GetAsync(predicate: x => x.Kod == request.Kod, cancellationToken: cancellationToken);
                var placeholderTopOrganizasyon = new Organizasyon() { Id = topOrganizasyon.UstBirimId, Aciklama_TR = "Top Organizasyon" };
                OrganizasyonComposite topOrganizasyonComposite = new OrganizasyonComposite(placeholderTopOrganizasyon.Id, placeholderTopOrganizasyon.Aciklama_TR);

                return new GetOrganizasyonSelectListWithTabAndMenuResponse()
                {
                    MenuResponse = GetMenu(organizasyonlar, placeholderTopOrganizasyon, topOrganizasyonComposite),
                };
            }

            private OrganizasyonComposite GetMenu(List<Organizasyon> organizasyonlar, Organizasyon topOrganizasyon, OrganizasyonComposite topOrganizasyonComposite, OrganizasyonComposite last = null)
            {
                organizasyonlar.Where(organizasyon => organizasyon.UstBirimId == topOrganizasyon.Id).ToList().ForEach(organizasyon =>
                {
                    var organizasyonComposite = new OrganizasyonComposite(organizasyon.Id, organizasyon.Aciklama_TR);

                    if (last != null)
                    {
                        last.Add(organizasyonComposite);
                    }
                    else
                    {
                        topOrganizasyonComposite.Add(organizasyonComposite);
                    }

                    GetMenu(organizasyonlar, organizasyon, topOrganizasyonComposite, organizasyonComposite);
                });


                return topOrganizasyonComposite;
            }
        }
    }
}
