using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyUniversity.UserManager.MapperProfiles;
using MyUniversity.UserManager.Repository.DbContext;
using MyUniversity.UserManager.Services;
using MyUniversity.UserManager.Services.Implementation;
using MyUniversity.UserManager.Services.Settings;
using MyUniversity.UserManager.Settings;

namespace MyUniversity.UserManager.Extensions
{
    public static class HostExtensions
    {
        public static void AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UserManagerSettings>(configuration.GetSection(nameof(UserManagerSettings)));
            services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ITokenDecoder, TokenDecoder>();
            services.AddScoped<IRoleService, RoleService>();
        }

        public static void AddDbContext(this IServiceCollection services)
        {
            var dbSettings = services.BuildServiceProvider().GetService<IOptions<DatabaseSettings>>().Value;
            services.AddDbContext<UMDBContext>(options => options.UseSqlServer(dbSettings.ConnectionString));
            services.BuildServiceProvider().GetService<UMDBContext>().Database.Migrate();
        }

        public static void AddMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ControllerMapperProfile());
                mc.AddProfile(new ServiceMapperProfile());
                mc.AllowNullCollections = true;
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
