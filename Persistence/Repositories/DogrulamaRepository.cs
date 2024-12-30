using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class DogrulamaRepository : Repository<Dogrulama>, IDogrulamaRepository
    {
        public DogrulamaRepository(ModelContext context) : base(context)
        {
        }
    }
}
