using Application.Features.Dogrulamalar.Commands.Create;

namespace Application.Interfaces.Services
{
    public interface IDogrulamaService
    {
        Task<CreatedDogrulamaResponse> Gonder(int calisanId, string dogrulamaYontemi, CancellationToken cancellationToken);
    }
}
