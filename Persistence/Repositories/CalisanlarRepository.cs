using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class CalisanlarRepository : Repository<Calisan>, ICalisanlarRepository
    {
        public CalisanlarRepository(ModelContext context) : base(context)
        {
        }

        public async Task<string> GetGorevGrubuAsync(string tc)
        {
            if (string.IsNullOrEmpty(tc) || tc.Length != 11) throw new ClientsideException("TC Kimlik 11 haneli olmalıdır.");

            using var dbCommand = _context.Database.GetDbConnection().CreateCommand();

            dbCommand.CommandText = "select * from opuser.baskent_kimlik_temp bkt where (bkt.sinif_aciklama = 'TIP' or bkt.sinif_aciklama = 'DİŞ HEKİMLİĞİ' or bkt.sinif_aciklama = 'EĞİTİM ÖĞRETİM') and bkt.ozl_tckimlik_no = :tc and rownum = 1";
            dbCommand.Parameters.Add(new OracleParameter(":tc", tc));

            if (dbCommand.Connection.State != System.Data.ConnectionState.Open)
            {
                dbCommand.Connection.Open();
            }

            var result = await dbCommand.ExecuteScalarAsync();

            return result is null ? "İdari" : "Akademik";
        }

        public async Task<bool> BirimAmiriMiAsync(int id)
        {
            Calisan calisan = (await _context.Calisanlar.FirstAsync(x => x.Id == id));
            string tc = calisan.Tc;
            int birimId = calisan.BirimId;

            if (string.IsNullOrEmpty(tc) || tc.Length != 11) throw new ClientsideException("TC Kimlik 11 haneli olmalıdır.");

            using var dbCommand = _context.Database.GetDbConnection().CreateCommand();

            dbCommand.CommandText = "SELECT DISTINCT BK.OZL_TCKIMLIK_NO, BK.OZL_AD || ' ' || BK.OZL_SOYAD ISIM_SOYISIM, OPUSER.GET_BOLUM_ADI(BU.BOLUM_KODU), BUG.TR_TANIM FROM OPUSER.BOLUMLER_YETKILI BU, OPUSER.BASKENT_KIMLIK BK, OPUSER.BOLUMLER_YETKILI_GOREV_TANIM BUG, OPUSER.BOLUM_SEVIYE BS WHERE BU.IDARI_GOREV = BUG.IDARI_GOREV AND BK.OZL_TCKIMLIK_NO = :tc AND BU.TC_KIMLIK_NO = BK.OZL_TCKIMLIK_NO AND BK.GOREV_YER_ID = :birimId AND BS.BOLUM_ADI_TR LIKE '%' || BK.GOREV_YER_ACIKLAMA || '%' AND BU.BOLUM_KODU = BS.KOD";
            dbCommand.Parameters.Add(new OracleParameter(":tc", tc));
            dbCommand.Parameters.Add(new OracleParameter(":birimId", birimId));


            if (dbCommand.Connection.State != System.Data.ConnectionState.Open)
            {
                dbCommand.Connection.Open();
            }

            var result = await dbCommand.ExecuteScalarAsync();

            return result != null;
        }
    }
}
