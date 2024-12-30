using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;
using System.Reflection;

namespace Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ModelContext>(options =>
            {
                options.UseOracle(connectionString, option =>
                {
                    option.MigrationsAssembly(Assembly.GetAssembly(typeof(ModelContext)).GetName().Name);
                });
            });

            services.AddScoped<IIzinlerRepository, IzinlerRepository>();

            services.AddScoped<ICalisanlarRepository, CalisanlarRepository>();

            services.AddScoped<IIzinTurleriRepository, IzinTurleriRepository>();

            services.AddScoped<IIzinGruplariRepository, IzinGruplariRepository>();

            services.AddScoped<IIzinHareketleriRepository, IzinHareketleriRepository>();

            services.AddScoped<IIzinGrupIzinOnayTanimRepository, IzinGrupIzinOnayTanimRepository>();

            services.AddScoped<IIzinOnayTanimCalisanRepository, IzinOnayTanimCalisanRepository>();

            services.AddScoped<IBaskentResimBlobRepository, BaskentResimBlobRepository>();

            services.AddScoped<ITatillerRepository, TatillerRepository>();

            services.AddScoped<IYetkililerRepository, YetkililerRepository>();

            services.AddScoped<IRetSebepleriRepository, RetSebepleriRepository>();

            services.AddScoped<IRetDetaylariRepository, RetDetaylariRepository>();

            services.AddScoped<IIzinOnayTanimlariRepository, IzinOnayTanimlariRepository>();

            services.AddScoped<IBirimlerRepository, BirimlerRepository>();

            services.AddScoped<IIsyerleriRepository, IsyerleriRepository>();

            services.AddScoped<IUnvanlarRepository, UnvanlarRepository>();

            services.AddScoped<IDogrulamaRepository, DogrulamaRepository>();

            services.AddScoped<IOrganizasyonlarRepository, OrganizasyonSemasiRepository>();

            services.AddScoped<IOrganizasyonHareketleriRepository, OrganizasyonHareketleriRepository>();

            return services;
        }
    }
}
