using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class RetSebepleriRepository : Repository<RetSebep>, IRetSebepleriRepository
    {
        public RetSebepleriRepository(ModelContext context) : base(context)
        {
        }
    }
}
