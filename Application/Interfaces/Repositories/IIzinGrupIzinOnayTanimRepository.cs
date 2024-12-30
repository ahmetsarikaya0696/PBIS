using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IIzinGrupIzinOnayTanimRepository : IAsyncRepository<IzinGrupIzinOnayTanim>
    {
        Task<int> GetIzinOnayTanimIdAsync(int izinGrupId, int onayTanimSirasi, CancellationToken cancellationToken);
    }
}
