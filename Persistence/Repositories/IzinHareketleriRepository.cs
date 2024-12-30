using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinHareketleriRepository : Repository<IzinHareket>, IIzinHareketleriRepository
    {
        public IzinHareketleriRepository(ModelContext context) : base(context)
        {
        }
    }
}
