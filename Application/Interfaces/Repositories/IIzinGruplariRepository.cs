using Application.Features.IzinGruplari.Commands.Create;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IIzinGruplariRepository : IAsyncRepository<IzinGrup>
    {
        Task<int> GetIzinGrupIdAsync(int calisanId, int unvanId, int birimId, int isyeriId, CancellationToken cancellationToken);
        Task<bool> UpdateIzinGrupWithIzinOnayTanimlariAsync(List<IzinOnayTanimIdWithSira> ızinOnayTanimIdWithSiralar, IzinGrup izinGrup);
    }
}
