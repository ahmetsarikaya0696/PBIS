using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class IzinlerRepository : Repository<Izin>, IIzinlerRepository
    {
        public IzinlerRepository(ModelContext context) : base(context)
        {

        }

        public async Task<int> GetKalanIzinGunSayisiByTcAsync(string tc)
        {
            if (string.IsNullOrEmpty(tc) || tc.Length != 11) throw new ClientsideException("TC Kimlik 11 haneli olmalıdır.");

            using var dbCommand = _context.Database.GetDbConnection().CreateCommand();

            dbCommand.CommandText = "SELECT A.TOPLAM_KALAN_IZNI FROM BASKENT_PERSONEL_IZIN_HAK@LOGO A WHERE A.TC_KIMLIK_NO =:tc";
            dbCommand.Parameters.Add(new OracleParameter(":tc", tc));

            if (dbCommand.Connection.State != System.Data.ConnectionState.Open)
            {
                dbCommand.Connection.Open();
            }

            var result = await dbCommand.ExecuteScalarAsync();

            if (result == null) throw new ClientsideException("Kalan izin günü verisi bulunamadı!");

            int.TryParse(result.ToString(), out int toplamKalanIzin);

            return toplamKalanIzin;
        }

        public async Task<int> GetIzinGunSayisiAsync(DateTime baslangicTarihi, DateTime bitisTarihi, int izinTurId, CancellationToken cancellationToken)
        {
            IzinTur izinTur = await _context.IzinTurleri.FirstOrDefaultAsync(predicate: x => x.Id == izinTurId, cancellationToken: cancellationToken);

            // Sabit izin gün sayısı varsa
            if (izinTur.SabitGunSayisi != null)
                return izinTur.SabitGunSayisi.Value;

            // Başlangıç Bitiş arasındaki gün sayısını hesapla
            TimeSpan zamanFarkı = bitisTarihi - baslangicTarihi;
            int gunSayisi = zamanFarkı.Days + 1;

            // Tatil günü kullanılan babalık izni izin günü olarak sayılır
            if (izinTur.TatilGunleriSayilir)
                return gunSayisi;

            for (DateTime date = baslangicTarihi; date <= bitisTarihi; date = date.AddDays(1))
            {
                bool tatilGunu = await _context.Tatiller.AnyAsync(predicate: x => x.Tarih == date, cancellationToken: cancellationToken);

                if (date.DayOfWeek == DayOfWeek.Sunday || tatilGunu)
                    gunSayisi--;
            }

            return gunSayisi;
        }
    }
}
