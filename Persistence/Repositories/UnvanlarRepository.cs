using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class UnvanlarRepository : Repository<Unvan>, IUnvanlarRepository
    {
        public UnvanlarRepository(ModelContext context) : base(context)
        {
        }
    }
}
