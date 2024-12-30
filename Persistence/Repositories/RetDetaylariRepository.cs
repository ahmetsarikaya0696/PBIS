using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class RetDetaylariRepository : Repository<RetDetay>, IRetDetaylariRepository
    {
        public RetDetaylariRepository(ModelContext context) : base(context)
        {
        }
    }
}
