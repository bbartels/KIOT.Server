using System;
using System.Net;
using Microsoft.EntityFrameworkCore;

using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence
{
    internal class KIOTContext : DbContext
    {
        public KIOTContext(DbContextOptions<KIOTContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> Users { get; private set; }
        public DbSet<RefreshToken> RefreshTokens { get; private set; }
        public DbSet<Caretaker> Caretakers { get; private set; }
        public DbSet<Customer> Customers { get; private set; }

        public DbSet<CaretakerForCustomer> IsCaredForBys { get; private set; }
        public DbSet<CaretakerForCustomerRequest> CaretakerForCustomerRequests { get; private set; }

        public DbSet<CustomerAppliance> CustomerAppliances { get; private set; }
        public DbSet<ApplianceCategory> ApplianceCategories { get; private set; }
        public DbSet<ApplianceType> ApplianceTypes { get; private set; }

        public DbSet<MobileDevice> MobileDevices { get; private set; }
        public DbSet<PushToken> PushTokens { get; private set; }
        public DbSet<CustomerTask> CustomerTasks { get; private set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SetupUsers(builder);
            SetupRefreshTokens(builder);
            SetupCaretakerForCustomer(builder);
            SetupCaretakerForCustomerRequests(builder);
            SetupAppliances(builder);
            SetupMobileDevices(builder);
            SetupCustomerTasks(builder);

            SeedData(builder);
        }

        private static void SetupUsers(ModelBuilder builder)
        {
            builder.Entity<User>()
                .Metadata
                .FindNavigation(nameof(User.RefreshTokens))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Entity<User>()
                .Ignore(u => u.Claims)
                .Ignore(u => u.HashedPassword)
                .Ignore(u => u.Email);

            builder.Entity<Customer>()
                .HasBaseType<User>();

            builder.Entity<Customer>()
                .HasIndex(c => c.Guid)
                .IsUnique();

            builder.Entity<Customer>()
                .HasIndex(c => c.Code)
                .IsUnique();

            builder.Entity<Customer>()
                .Property(c => c.Code)
                .HasMaxLength(16);

            builder.Entity<Caretaker>()
                .HasBaseType<User>();

            builder.Entity<Caretaker>()
                .HasIndex(c => c.Guid)
                .IsUnique();
        }

        private static void SetupRefreshTokens(ModelBuilder builder)
        {
            builder.Entity<RefreshToken>()
                .Property(rt => rt.RemoteAddress)
                .HasConversion(val => val.ToString(), val => IPAddress.Parse(val));

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Guid)
                .IsUnique();
        }

        private static void SetupCaretakerForCustomerRequests(ModelBuilder builder)
        {
            builder.Entity<CaretakerForCustomerRequest>()
                .HasIndex(cf => cf.Guid)
                .IsUnique();

            builder.Entity<CaretakerForCustomerRequest>()
                .HasIndex(cf => new { cf.CaretakerId, cf.CustomerId })
                .IsUnique();

            builder.Entity<CaretakerForCustomerRequest>()
                .HasOne(cf => cf.Customer)
                .WithMany(c => c.IsCaredForByRequests)
                .HasForeignKey(cf => cf.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaretakerForCustomerRequest>()
                .HasOne(cf => cf.Caretaker)
                .WithMany(c => c.TakingCareOfRequests)
                .HasForeignKey(cf => cf.CaretakerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetupCaretakerForCustomer(ModelBuilder builder)
        {
            builder.Entity<CaretakerForCustomer>()
                .HasOne(cf => cf.Customer)
                .WithMany(c => c.IsCaredForBy)
                .HasForeignKey(cf => cf.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaretakerForCustomer>()
                .HasOne(cf => cf.Caretaker)
                .WithMany(ct => ct.TakingCareOf)
                .HasForeignKey(cf => cf.CaretakerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaretakerForCustomer>()
                .HasIndex(cf => new { cf.CaretakerId, cf.CustomerId })
                .IsUnique();
        }

        private static void SetupCustomerTasks(ModelBuilder builder)
        {
            builder.Entity<CustomerTask>()
                .HasOne(ct => ct.Customer)
                .WithMany(c => c.Tasks)
                .HasForeignKey(ct => ct.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerTask>()
                .HasOne(ct => ct.Caretaker)
                .WithMany(c => c.CustomersTasks)
                .HasForeignKey(ct => ct.CaretakerId);
        }

        private static void SetupMobileDevices(ModelBuilder builder)
        {
            builder.Entity<MobileDeviceForUser>()
                .HasOne(c => c.User)
                .WithMany(mb => mb.UsesDevices)
                .HasForeignKey(mb => mb.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MobileDeviceForUser>()
                .HasOne(c => c.MobileDevice)
                .WithMany(mb => mb.UsedBy)
                .HasForeignKey(mb => mb.MobileDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MobileDeviceForUser>()
                .HasIndex(mfc => new { mfc.MobileDeviceId, mfc.UserId })
                .IsUnique();

            builder.Entity<MobileDevice>()
                .HasIndex(mb => mb.InstallationId)
                .IsUnique();

            builder.Entity<MobileDevice>()
                .Property(mb => mb.MobileOS)
                .HasConversion(
                    v => v.ToString(),
                    v => (MobileOS) Enum.Parse(typeof(MobileOS), v));

            builder.Entity<PushToken>()
                .HasOne(pt => pt.MobileDevice)
                .WithMany(d => d.PushTokens)
                .HasForeignKey(pt => pt.MobileDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PushToken>()
                .HasOne(pt => pt.User)
                .WithMany(c => c.PushTokens)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Todo: Maybe PushToken unique??
        }

        private static void SetupAppliances(ModelBuilder builder)
        {
            builder.Entity<ApplianceType>()
                .HasIndex(at => at.ApplianceTypeId)
                .IsUnique();

            builder.Entity<ApplianceType>()
                .HasMany(at => at.CustomerAppliances)
                .WithOne(c => c.ApplianceType)
                .HasForeignKey(at => at.ApplianceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerAppliance>()
                .HasIndex(ca => ca.ApplianceId)
                .IsUnique();

            builder.Entity<CustomerAppliance>()
                .HasOne(ca => ca.ApplianceType)
                .WithMany(at => at.CustomerAppliances)
                .HasForeignKey(ca => ca.ApplianceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasMany(c => c.Appliances)
                .WithOne(ca => ca.Customer)
                .HasForeignKey(c => c.CustomerId);

            builder.Entity<Customer>()
                .HasMany(c => c.ApplianceCategories)
                .WithOne(ca => ca.Customer)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomerAppliance>()
                .HasOne(ca => ca.Category)
                .WithMany(at => at.Appliances)
                .HasForeignKey(ca => ca.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }

        private static void SeedData(ModelBuilder builder)
        {
            builder.Entity<Customer>()
                .HasData(ApplicationDbInitializer.GetCustomers());

            builder.Entity<Caretaker>()
                .HasData(ApplicationDbInitializer.GetCaretakers());

            builder.Entity<CaretakerForCustomerRequest>()
                .HasData(ApplicationDbInitializer.GetCaretakerRequests());
        }
    }
}
