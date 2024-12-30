using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class OrganizasyonHareketleriRepository : Repository<OrganizasyonHareket>, IOrganizasyonHareketleriRepository
    {
        public OrganizasyonHareketleriRepository(ModelContext context) : base(context)
        {
        }
    }
}
