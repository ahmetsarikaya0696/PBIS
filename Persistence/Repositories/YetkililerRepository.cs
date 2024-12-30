using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class YetkililerRepository : Repository<Yetkili>, IYetkililerRepository
    {
        public YetkililerRepository(ModelContext context) : base(context)
        {
        }
    }
}
