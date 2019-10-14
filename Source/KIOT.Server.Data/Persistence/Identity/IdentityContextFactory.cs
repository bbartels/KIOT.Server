using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KIOT.Server.Data.Persistence.Identity
{
    internal class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            var builder = new DbContextOptionsBuilder<IdentityContext>();
            builder.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));

            return new IdentityContext(builder.Options);
        }
    }
}
