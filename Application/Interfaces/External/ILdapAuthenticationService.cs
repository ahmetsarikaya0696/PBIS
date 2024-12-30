namespace Application.Interfaces.External
{
    public interface ILdapAuthenticationService
    {
        Task<bool> ValidateCredentialsAsync(string email, string password);
    }
}
