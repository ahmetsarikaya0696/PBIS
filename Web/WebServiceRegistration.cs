using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;
using Web.Filters;

namespace Web
{
    public static class WebServiceRegistration
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddSession();

            services.AddHttpContextAccessor();

            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/giris";
                        options.ExpireTimeSpan = TimeSpan.FromHours(1);
                        options.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = context =>
                            {
                                context.RedirectUri = context.Request.PathBase + options.LoginPath;
                                context.Response.Redirect(context.RedirectUri);
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<CheckFotografActionFilter>();

            services.AddScoped<IpCheckFilter>();

            services.AddScoped<JsonExceptionFilter>();

            services.AddScoped<AuthorizeRaporActionFilter>();

            return services;
        }
    }
}
