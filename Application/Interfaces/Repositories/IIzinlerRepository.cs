using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IIzinlerRepository : IAsyncRepository<Izin>
    {
        Task<int> GetKalanIzinGunSayisiByTcAsync(string tc);
        Task<int> GetIzinGunSayisiAsync(DateTime baslangicTarihi, DateTime bitisTarihi, int izinTurId, CancellationToken cancellationToken);
    }
}
