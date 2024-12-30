using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class BaskentResimBlobRepository : Repository<BaskentResimBlob>, IBaskentResimBlobRepository
    {
        public BaskentResimBlobRepository(ModelContext context) : base(context)
        {
        }
    }
}
