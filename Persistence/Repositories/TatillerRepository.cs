using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class TatillerRepository : Repository<Tatil>, ITatillerRepository
    {
        public TatillerRepository(ModelContext context) : base(context)
        {
        }
    }
}
