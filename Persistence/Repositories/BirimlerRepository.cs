using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class BirimlerRepository : Repository<Birim>, IBirimlerRepository
    {
        public BirimlerRepository(ModelContext context) : base(context)
        {
        }
    }
}
