using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IsyerleriRepository : Repository<Isyeri>, IIsyerleriRepository
    {
        public IsyerleriRepository(ModelContext context) : base(context)
        {
        }
    }
}
