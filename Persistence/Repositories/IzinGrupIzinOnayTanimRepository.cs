using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinGrupIzinOnayTanimRepository : Repository<IzinGrupIzinOnayTanim>, IIzinGrupIzinOnayTanimRepository
    {
        public IzinGrupIzinOnayTanimRepository(ModelContext context) : base(context)
        {
        }

        public async Task<int> GetIzinOnayTanimIdAsync(int izinGrupId, int onayTanimSirasi, CancellationToken cancellationToken)
        {
            IzinGrupIzinOnayTanim izinGrupIzinOnayTanim = await GetAsync(predicate: x => x.IzinGrupId == izinGrupId &&
                                                                                    x.OnayTanimSirasi == onayTanimSirasi,
                                                                         cancellationToken: cancellationToken)
                           ?? throw new ClientsideException($"Onay tanım sırası {onayTanimSirasi} ve izin grup Id ' si {izinGrupId} olan veri bulunamadı!");

            int onayTanimId = izinGrupIzinOnayTanim.IzinOnayTanimId;

            return onayTanimId;
        }
    }
}
