using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinOnayTanimCalisanRepository : Repository<IzinOnayTanimCalisan>, IIzinOnayTanimCalisanRepository
    {
        public IzinOnayTanimCalisanRepository(ModelContext context) : base(context)
        {
        }
    }
}
