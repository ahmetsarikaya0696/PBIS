using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinOnayTanimlariRepository : Repository<IzinOnayTanim>, IIzinOnayTanimlariRepository
    {
        private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;

        public IzinOnayTanimlariRepository(IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, ModelContext context) : base(context)
        {
            _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
        }

        public async Task<bool> CreateIzinOnayTanimWithCalisanlar(List<int> calisanIdleri, IzinOnayTanim izinOnayTanim)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                IzinOnayTanim eklenmisIzinOnayTanim = await AddAsync(izinOnayTanim);
                int eklenmisIzinOnayTanimId = eklenmisIzinOnayTanim.Id;

                foreach (var calisanId in calisanIdleri)
                {
                    await _izinOnayTanimCalisanRepository.AddAsync(new()
                    {
                        CalisanId = calisanId,
                        IzinOnayTanimId = eklenmisIzinOnayTanimId
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

        public async Task<bool> UpdateIzinOnayTanimWithCalisanlar(List<int> yeniCalisanIdleri, IzinOnayTanim izinOnayTanim, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                IzinOnayTanim updatedIzinOnayTanim = await UpdateAsync(izinOnayTanim);

                int izinOnayTanimId = updatedIzinOnayTanim.Id;

                List<int> dbCalisanIdleri = (await _izinOnayTanimCalisanRepository.GetAllAsync(predicate: x => x.IzinOnayTanimId == izinOnayTanimId,
                                                                                               cancellationToken: cancellationToken))
                                                .Select(x => x.CalisanId)
                                                .ToList();



                List<int> farkCalisanIdleri = yeniCalisanIdleri.Except(dbCalisanIdleri).Concat(dbCalisanIdleri.Except(yeniCalisanIdleri)).ToList();

                if (farkCalisanIdleri.Count > 0)
                {
                    foreach (var farkCalisanId in farkCalisanIdleri)
                    {
                        if (!yeniCalisanIdleri.Contains(farkCalisanId))
                        {
                            IzinOnayTanimCalisan izinOnayTanimCalisan = await _izinOnayTanimCalisanRepository.GetAsync(predicate: x => x.CalisanId == farkCalisanId &&
                                                                                                                                    x.IzinOnayTanimId == izinOnayTanimId,
                                                                                                                       cancellationToken: cancellationToken);
                            await _izinOnayTanimCalisanRepository.DeleteAsync(izinOnayTanimCalisan);
                        }
                        else
                        {
                            await _izinOnayTanimCalisanRepository.AddAsync(new IzinOnayTanimCalisan() { CalisanId = farkCalisanId, IzinOnayTanimId = izinOnayTanimId });
                        }
                    }
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
