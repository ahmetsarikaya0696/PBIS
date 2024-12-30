using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Services;
using System.Reflection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();

            services.AddDbContext<BildirimContext>(options =>
            {
                options.UseOracle(connectionString, option =>
                {
                    option.MigrationsAssembly(Assembly.GetAssembly(typeof(BildirimContext)).GetName().Name);
                });
            });

            services.AddScoped<ISmsService, SmsService>();

            services.AddScoped<IMailService, MailService>();

            return services;
        }
    }
}
