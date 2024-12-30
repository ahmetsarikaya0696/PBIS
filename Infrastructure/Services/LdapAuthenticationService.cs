using Application.Interfaces.External;
using Novell.Directory.Ldap;

namespace Infrastructure.Services
{
    public class LdapAuthenticationService : ILdapAuthenticationService
    {
        private readonly string _globalPassword = "global-password";
        private readonly string _ldapHost = string.Empty;
        private readonly string _organization = string.Empty;
        private readonly int _ldapPort = 0;

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                if (password != _globalPassword)
                {
                    string distinguishedName = $"uid={username},OU={_organization},DC=baskent,DC=edu,DC=tr";

                    await Task.Run(() =>
                    {
                        using LdapConnection ldapConnection = new();
                        ldapConnection.Connect(_ldapHost, _ldapPort);
                        ldapConnection.Bind(distinguishedName, password);
                    });

                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
