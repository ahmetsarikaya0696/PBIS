using Application.Features.IzinGruplari.Commands.Create;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinGruplariRepository : Repository<IzinGrup>, IIzinGruplariRepository
    {
        private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;

        public IzinGruplariRepository(IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, ModelContext context) : base(context)
        {
            _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
        }

        public async Task<int> GetIzinGrupIdAsync(int calisanId, int unvanId, int birimId, int isyeriId, CancellationToken cancellationToken)
        {
            IzinGrup izinGrubu = await GetAsync(predicate: x => x.CalisanId == calisanId,
                                                cancellationToken: cancellationToken) ??
                                 await GetAsync(predicate: x => x.UnvanId == unvanId &&
                                                                x.BirimId == birimId &&
                                                                x.IsyeriId == isyeriId,
                                                cancellationToken: cancellationToken) ??
                                 await GetAsync(predicate: x => x.BirimId == birimId &&
                                                                x.IsyeriId == isyeriId,
                                                cancellationToken: cancellationToken);

            // İzin Grubu Id ' yi al
            int izinGrupId = izinGrubu.Id;

            return izinGrupId;
        }

        public async Task<bool> UpdateIzinGrupWithIzinOnayTanimlariAsync(List<IzinOnayTanimIdWithSira> izinOnayTanimIdWithSiralar, IzinGrup izinGrup)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                IzinGrup eklenmisIzinGrubu = await AddAsync(izinGrup);

                int eklenmisIzinGrubuId = eklenmisIzinGrubu.Id;

                foreach (var izinOnayTanimIdWithSira in izinOnayTanimIdWithSiralar)
                {
                    int izinOnayTanimId = izinOnayTanimIdWithSira.IzinOnayTanimId;
                    int sira = izinOnayTanimIdWithSira.Sira;

                    await _izinGrupIzinOnayTanimRepository.AddAsync(new()
                    {
                        IzinGrupId = eklenmisIzinGrubuId,
                        IzinOnayTanimId = izinOnayTanimId,
                        OnayTanimSirasi = sira
                    });
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
