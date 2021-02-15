using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyUniversity.UserManager.Api.Settings;

namespace MyUniversity.UserManager.Api.Extensions
{
    public static class HostExtensions
    {
        public static void AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserManagerSettings>(configuration.GetSection(nameof(UserManagerSettings)));
            services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            //services.AddSingleton<IMessageClient, MessageClient>();

        }
    }
}
