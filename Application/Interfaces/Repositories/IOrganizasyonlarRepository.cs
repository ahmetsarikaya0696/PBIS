using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IOrganizasyonlarRepository : IAsyncRepository<Organizasyon>
    {
        Task<List<Organizasyon>> AltBirimleriBulAsync(int? id, CancellationToken cancellationToken);
    }
}
