using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinTurleriRepository : Repository<IzinTur>, IIzinTurleriRepository
    {
        public IzinTurleriRepository(ModelContext context) : base(context)
        {
        }
    }
}
