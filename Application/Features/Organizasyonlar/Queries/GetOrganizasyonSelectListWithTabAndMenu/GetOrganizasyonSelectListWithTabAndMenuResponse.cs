using Application.Features.Organizasyonlar.Composite;

namespace Application.Features.Organizasyonlar.Queries.GetOrganizasyonSelectListWithTabAndMenu
{
    public class GetOrganizasyonSelectListWithTabAndMenuResponse
    {
        public OrganizasyonComposite MenuResponse { get; set; }
        public List<GetOrganizasyonSelectListWithTabResponse> SelectListResponse => MenuResponse.Components.SelectMany(x => ((OrganizasyonComposite)x).GetSelectList()).ToList();
    }
}
