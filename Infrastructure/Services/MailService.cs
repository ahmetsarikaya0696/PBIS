using Application.Interfaces.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Persistence.Services
{
    public class MailService : IMailService
    {
        private readonly BildirimContext _bildirimContext;

        public MailService(BildirimContext bildirimContext)
        {
            _bildirimContext = bildirimContext;
        }

        public async Task Send(string mesaj, string konu, string toAdres, string toKisi, string fromAdres = "pbs@baskent.edu.tr", string fromKisi = "Başkent Üniversitesi Personel Bilgi Sistemi")
        {
            using var dbCommand = _bildirimContext.Database.GetDbConnection().CreateCommand();

            dbCommand.CommandText = "declare v_from_adres varchar(250); v_from_sahis varchar(250); v_baslik varchar(250); v_adres varchar(250); v_sahis varchar(250); v_mesaj varchar(500); begin v_from_adres := :fromAdres; v_from_sahis := :fromKisi; v_baslik := :konu; v_adres := :toAdres; v_sahis := :toKisi; v_mesaj := :mesaj; opuser.eposta_gonder(v_from_adres,v_from_sahis,v_baslik,v_adres,v_sahis,v_mesaj); end;";

            dbCommand.Parameters.Add(new OracleParameter(":fromAdres", fromAdres));
            dbCommand.Parameters.Add(new OracleParameter(":fromKisi", fromKisi));
            dbCommand.Parameters.Add(new OracleParameter(":konu", konu));
            dbCommand.Parameters.Add(new OracleParameter(":toAdres", toAdres));
            dbCommand.Parameters.Add(new OracleParameter(":toKisi", toKisi));
            dbCommand.Parameters.Add(new OracleParameter(":mesaj", mesaj));

            if (dbCommand.Connection.State != System.Data.ConnectionState.Open)
            {
                dbCommand.Connection.Open();
            }

            var result = await dbCommand.ExecuteScalarAsync();
        }
    }
}
