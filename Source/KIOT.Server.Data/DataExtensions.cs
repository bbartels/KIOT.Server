using System;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Services;
using KIOT.Server.Data.Configuration;
using KIOT.Server.Data.Persistence;
using KIOT.Server.Data.Persistence.Application;
using KIOT.Server.Data.Persistence.Application.Caretaker;
using KIOT.Server.Data.Persistence.Identity;

namespace KIOT.Server.Data
{
    public static class DataExtensions
    {
        internal static bool TestingEnvironment = false;

        public static IServiceCollection RegisterDataServices(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();

            var env = provider.GetService<IHostEnvironment>();

            if (!TestingEnvironment && env.EnvironmentName != "Testing")
            {
                services.AddDbContext<KIOTContext>(opt => opt.UseSqlServer(config.GetConnectionString("KIOTConnection")))
                        .AddDbContext<IdentityContext>(opt => opt.UseSqlServer(config.GetConnectionString("IdentityConnection")));

                provider = services.BuildServiceProvider();
                provider.GetService<KIOTContext>().ApplyMigration();
                provider.GetService<IdentityContext>().ApplyMigration();
            }

            services.RegisterIdentity();

            services.AddTransient(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerApplianceRepository, CustomerApplianceRepository>();
            services.AddTransient<IApplianceTypeRepository, ApplianceTypeRepository>();
            services.AddTransient<ICaretakerRepository, CaretakerRepository>();
            services.AddTransient<ICaretakerForCustomerRepository, CaretakerForCustomerRepository>();
            services.AddTransient<ICaretakerForCustomerRequestRepository, CaretakerForCustomerRequestRepository>();
            services.AddTransient<IMobileDeviceRepository, MobileDeviceRepository>();

            services.AddSingleton<ITokenConfiguration, TokenConfiguration>();

            return services;
        }

        private static void ApplyMigration(this DbContext context) { if (context.Database.GetPendingMigrations().Any()) { context.Database.Migrate(); } }

        private static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            var identityBuilder = services.AddIdentityCore<ApplicationUser>(ib =>
            {
                ib.Password.RequireDigit = false;
                ib.Password.RequireNonAlphanumeric= false;
                ib.Password.RequireUppercase = false;
                ib.Password.RequiredLength = 6;
            });

            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<IdentityContext>();

            return services;
        }
    }
}
