using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using KIOT.Server.Api.Tests.Integration.MockServices;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Services;
using KIOT.Server.Data;
using KIOT.Server.Data.Persistence;
using KIOT.Server.Data.Persistence.Identity;

namespace KIOT.Server.Api.Tests.Integration
{
    public class InMemoryKIOTApiFactory<T> : WebApplicationFactory<Startup>
    {
        private IServiceProvider _provider;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                DataExtensions.TestingEnvironment = true;

                var provider = new ServiceCollection()
                     .AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<KIOTContext>(opt =>
                {
                    opt.UseInMemoryDatabase("InMemoryKIOT");
                    opt.UseInternalServiceProvider(provider);
                });

                services.AddDbContext<IdentityContext>(opt =>
                {
                    opt.UseInMemoryDatabase("InMemoryIdentity");
                    opt.UseInternalServiceProvider(provider);
                });

                services.AddSingleton<IStartupConfigureServicesFilter>(
                    new ConfigureTestServicesFilter(TestServices));

                void TestServices(IServiceCollection s)
                {
                    s.AddSingleton<IHttpRequestService, MockHttpRequestService>();
                    s.AddSingleton<IApplianceServiceConfiguration, MockApplianceServiceConfiguration>();
                    s.AddSingleton<IApiConfiguration, TestApiConfiguration>();
                }

                _provider = services.BuildServiceProvider();
                var env = _provider.GetService<IHostingEnvironment>();
                env.EnvironmentName = "Testing";

                SeedDbContext();
            });
        }

        internal void SeedDbContext()
        {
            using (var scope = _provider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var identityDb = scopedServices.GetRequiredService<IdentityContext>();
                var kiotDb = scopedServices.GetRequiredService<KIOTContext>();

                kiotDb.Database.EnsureDeleted();
                kiotDb.Database.EnsureCreated();

                identityDb.Database.EnsureDeleted();
                identityDb.Database.EnsureCreated();
            }
        }

        internal KIOTContext GetKIOTContext()
        {
            var scope = _provider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            return scopedServices.GetRequiredService<KIOTContext>();
        }

        internal IdentityContext GetIdentityContext()
        {
            var scope = _provider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            return scopedServices.GetRequiredService<IdentityContext>();
        }
    }
}
