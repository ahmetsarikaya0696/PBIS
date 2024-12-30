using Application;
using Infrastructure;
using Persistence;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ygdb5");

            // Add services to the container.
            builder.Services.AddApplicationServices();

            var bildirimConnectionString = builder.Configuration.GetConnectionString("bildirim");

            builder.Services.AddPersistenceServices(connectionString);

            builder.Services.AddInfrastructureServices(bildirimConnectionString);

            builder.Services.AddUIServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}