using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class OrganizasyonSemasiRepository : Repository<Organizasyon>, IOrganizasyonlarRepository
    {
        public OrganizasyonSemasiRepository(ModelContext context) : base(context)
        {
        }

        public async Task<List<Organizasyon>> AltBirimleriBulAsync(int? id, CancellationToken cancellationToken)
        {
            var altBirimler = new List<Organizasyon>();

            var altlar = await _context.OrganizasyonSemasi.Where(predicate: x => x.Aktif &&
                                                                             x.UstBirimId == id)
                                                           .ToListAsync(cancellationToken: cancellationToken);

            foreach (var alt in altlar)
            {
                altBirimler.Add(alt);
                var childItems = await AltBirimleriBulAsync(alt.Id, cancellationToken);
                altBirimler.AddRange(childItems);
            }

            return altBirimler.DistinctBy(x => x.Id).ToList();
        }


        public override async Task<Organizasyon> AddAsync(Organizasyon entity)
        {
            await SetKodForEntityAsync(entity);
            await base.AddAsync(entity);
            return entity;
        }

        public override async Task<Organizasyon> UpdateAsync(Organizasyon entity)
        {
            if (entity.UstBirimId == 0)
            {
                await base.UpdateAsync(entity);
                return entity;
            }

            string oldEntityKod = entity.Kod;

            // Set the new Kod for the entity
            await SetKodForEntityAsync(entity);
            await base.UpdateAsync(entity);

            // Update child nodes
            await UpdateChildNodesKodAsync(oldEntityKod, entity.Kod);

            return entity;
        }

        private async Task SetKodForEntityAsync(Organizasyon entity)
        {
            // Id ' sini bul
            var siblingNodeKodlari = await _context.OrganizasyonSemasi
                                               .Where(x => x.UstBirimId == entity.UstBirimId && x.Id != entity.Id)
                                               .Select(x => x.Kod)
                                               .ToListAsync();

            var ustBirim = await _context.OrganizasyonSemasi.FirstOrDefaultAsync(x => x.Id == entity.UstBirimId);
            string newUstBirimKod = ustBirim != null ? ustBirim.Kod : null;

            if (siblingNodeKodlari == null || siblingNodeKodlari.Count == 0)
            {
                entity.Kod = entity.UstBirimId != 0 ? $"{newUstBirimKod}.1" : $"1";
            }
            else
            {
                var indicator = siblingNodeKodlari.Select(x => Convert.ToInt32(x.Split(".")[^1])).Max() + 1;

                entity.Kod = entity.UstBirimId != 0 ? $"{newUstBirimKod}.{indicator}" : $"{indicator}";
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateChildNodesKodAsync(string oldKod, string newKod)
        {
            var childNodes = await _context.OrganizasyonSemasi.Where(x => x.Kod.StartsWith(oldKod) && x.Kod.Length > oldKod.Length).ToListAsync();

            foreach (var childNode in childNodes)
            {
                string part1 = newKod;
                string part2 = GetSecondPart(childNode.Kod, oldKod);
                childNode.Kod = $"{part1}.{part2}";
                await base.UpdateAsync(childNode);
            }
        }

        private string GetSecondPart(string childNodeKod, string parentNodeKod)
        {
            string[] ipParts = childNodeKod.Split('.');

            int firstPartEnd = Array.IndexOf(ipParts, parentNodeKod.Split('.').Last()) + 1;

            if (firstPartEnd == 0)
            {
                return childNodeKod;
            }

            string secondPart = string.Join(".", ipParts, firstPartEnd, ipParts.Length - firstPartEnd);

            return secondPart;
        }


    }
}
