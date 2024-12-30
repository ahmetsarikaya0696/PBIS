namespace Application.Interfaces.Repositories
{
    public interface ISmsService
    {
        Task Send(string mesaj, string tc);
    }
}
