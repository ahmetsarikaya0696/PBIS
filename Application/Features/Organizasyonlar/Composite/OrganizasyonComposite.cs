using Application.Features.Organizasyonlar.Queries.GetOrganizasyonSelectListWithTabAndMenu;
using System.Text;
using System.Web;

namespace Application.Features.Organizasyonlar.Composite
{
    public class OrganizasyonComposite : IComponent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<IComponent> Components { get; private set; } = new();

        public OrganizasyonComposite(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Add(IComponent component)
        {
            Components.Add(component);
        }

        public void Remove(IComponent component)
        {
            Components.Remove(component);
        }

        public int Count()
        {
            return Components.Sum(x => x.Count() == 0 ? 1 : x.Count());
        }

        public string Display()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"<div class='d-flex align-items-center gap-1 my-1'>");

            if (Count() > 0)
            {
                stringBuilder.Append(@"<i class='ki-duotone ki-minus-square toggle fs-1'>
                                             <span class='path1'></span>
                                             <span class='path2'></span>
                                             <span class='path3'></span>
                                       </i>");
            }
            else if (Count() == 0)
            {
                stringBuilder.Append(@"<i class='ki-duotone ki-arrow-right fs-1'>
                                             <span class='path1'></span>
                                             <span class='path2'></span>
                                       </i>");
            }

            if (Count() != 0)
            {
                stringBuilder.Append($"<span class='menu fs-6'><strong>{Name} ({Count()})</strong></span>");
            }
            else
            {
                stringBuilder.Append($"<span class='menu fs-6'>{Name}</span>");
            }

            stringBuilder.Append("</div>");

            if (!Components.Any()) return stringBuilder.ToString();

            stringBuilder.Append("<ul class='list-group list-group-flush ms-3'>");

            foreach (var component in Components)
            {
                stringBuilder.Append(component.Display());
            }

            stringBuilder.Append("</ul>");

            return stringBuilder.ToString();
        }

        public List<GetOrganizasyonSelectListWithTabResponse> GetSelectList(string space = "")
        {
            space = HttpUtility.HtmlDecode(space);

            var list = new List<GetOrganizasyonSelectListWithTabResponse>() { new() { Text = $"{space}{Name}", Value = Id.ToString() } };

            if (Components.Any(x => x is OrganizasyonComposite))
            {
                space += "&nbsp;&nbsp;&nbsp;&nbsp;";
                Components.ToList().ForEach(x =>
                {
                    if (x is OrganizasyonComposite organizasyonComposite)
                    {
                        list.AddRange(organizasyonComposite.GetSelectList(space));
                    }
                });
            }

            return list;
        }
    }
}
