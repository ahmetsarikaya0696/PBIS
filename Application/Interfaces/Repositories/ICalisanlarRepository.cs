using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICalisanlarRepository : IAsyncRepository<Calisan>
    {
        Task<string> GetGorevGrubuAsync(string tc);
        Task<bool> BirimAmiriMiAsync(int id);
    }
}
