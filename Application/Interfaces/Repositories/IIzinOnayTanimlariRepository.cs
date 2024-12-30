using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IIzinOnayTanimlariRepository : IAsyncRepository<IzinOnayTanim>
    {
        Task<bool> CreateIzinOnayTanimWithCalisanlar(List<int> calisanIdleri, IzinOnayTanim izinOnayTanim);
        Task<bool> UpdateIzinOnayTanimWithCalisanlar(List<int> yeniCalisanIdleri, IzinOnayTanim izinOnayTanim, CancellationToken cancellationToken);
    }
}
