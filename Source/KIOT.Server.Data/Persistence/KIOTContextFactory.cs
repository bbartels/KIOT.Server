using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KIOT.Server.Data.Persistence
{
    internal class KIOTContextFactory : IDesignTimeDbContextFactory<KIOTContext>
    {
        public KIOTContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();

            var builder = new DbContextOptionsBuilder<KIOTContext>();

            builder.UseSqlServer(configuration.GetConnectionString("KIOTConnection"));

            return new KIOTContext(builder.Options);
        }
    }
}
