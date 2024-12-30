namespace Application.Interfaces.Repositories
{
    public interface IMailService
    {
        Task Send(string mesaj, string konu, string toAdres, string toKisi, string fromAdres = "pbs@baskent.edu.tr", string fromKisi = "Başkent Üniversitesi Personel Bilgi Sistemi");
    }
}
