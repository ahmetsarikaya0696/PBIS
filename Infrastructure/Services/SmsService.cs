using Application.Interfaces.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Persistence.Services
{
    public class SmsService : ISmsService
    {
        private readonly BildirimContext _bildirimContext;

        public SmsService(BildirimContext bildirimContext)
        {
            _bildirimContext = bildirimContext;
        }

        public async Task Send(string mesaj, string tc)
        {
            using var dbCommand = _bildirimContext.Database.GetDbConnection().CreateCommand();

            dbCommand.CommandText = "DECLARE output VARCHAR(200); p_in_ileri_tarih VARCHAR(200) := NULL; p_hata_kodu VARCHAR(200) := NULL; p_hata_text VARCHAR(200) := NULL; BEGIN output := opuser.pkg_sms_yeni.smstopersonelbytckimlik(p_in_mesaj => :mesaj, p_in_tckimlik => :tc, p_in_ileri_tarih => p_in_ileri_tarih, p_hata_kodu => p_hata_kodu, p_hata_text => p_hata_text); END;";
            dbCommand.Parameters.Add(new OracleParameter(":mesaj", mesaj));
            dbCommand.Parameters.Add(new OracleParameter(":tc", tc));

            if (dbCommand.Connection.State != System.Data.ConnectionState.Open) dbCommand.Connection.Open();

            var etkilenenSatirSayisi = await dbCommand.ExecuteNonQueryAsync();
        }
    }
}
