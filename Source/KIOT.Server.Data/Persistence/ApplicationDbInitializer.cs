using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Data.Persistence.Identity;

namespace KIOT.Server.Data.Persistence
{
    internal static class ApplicationDbInitializer
    {
        private const string Password = "password";

        private static readonly IList<User> Users = new List<User>
        {
            new Customer("24cfabf2-30e5-4e59-9be1-88dd861dec3c",
                "Matt", "Wilson", "mwilson", "0055_ALT-CM0103", "+4418821000") { Id = -1, Guid = Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2") },
            new Customer("dfe6beec-0f05-4fe9-851e-078160950468",
                "Frank", "Brown", "fbrown", "0055_ALT-CM0104", "+4418821001") { Id = -2, Guid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557") },
            new Customer("586e130d-2583-4adb-b894-05304f68d1cf",
                "Will", "Jones", "wjones", "0055_ALT-CM0105", "+4418821002") { Id = -3, Guid = Guid.Parse("cf0415f9-ab05-47ba-a0ec-2c0e3f874bbc") },
            new Caretaker("0254effb-2ca2-4403-97c0-71065525ec3d",
                "Taylor", "Williams", "twilliams", "+4418821003") { Id = -4, Guid = Guid.Parse("9f746084-8796-4ff5-874e-75eb33bfe9e3") },
            new Caretaker("11fb19f4-e22c-4b0e-91ca-0917ad68ee80",
                "Jack", "Smith", "jsmith", "+4418821004") { Id = -5, Guid = Guid.Parse("912217dd-b725-47aa-96b9-8fd73e7e6fa0") },
            new Caretaker("be52b267-353c-45c5-9ab0-9592f5d8e3e0",
                "Jared", "Cole", "jcole", "+4418821005") { Id = -6, Guid = Guid.Parse("e0e26424-f4cd-4400-bc78-28b0bfef729a") }
        };

        private static readonly IList<CaretakerForCustomerRequest> CaretakerRequests = new List<CaretakerForCustomerRequest>
        {
            new CaretakerForCustomerRequest(-4, -1) { Id = -1, Guid = Guid.Parse("f61ee92d-792e-45dd-a664-309178a71830") },
            new CaretakerForCustomerRequest(-4, -2) { Id = -2, Guid = Guid.Parse("ccf08264-5f4b-48bc-9620-83257f14e6df") },
            new CaretakerForCustomerRequest(-4, -3) { Id = -3, Guid = Guid.Parse("4af29383-bb2e-4b3f-8645-ba1f6318ea0f") }
        };

        private static string GetEmail(string username) { return $"{username}@test.fake"; }

        private static string GetPassword() { return Password; }

        public static IEnumerable<User> GetUsers() { return Users; }

        public static IEnumerable<Customer> GetCustomers() { return Users.Where(x => x is Customer).Cast<Customer>(); }
        public static IEnumerable<Caretaker> GetCaretakers() { return Users.Where(x => x is Caretaker).Cast<Caretaker>(); }

        public static IEnumerable<CaretakerForCustomerRequest> GetCaretakerRequests() { return CaretakerRequests; }

        public static IEnumerable<ApplicationUser> GetAppUsers()
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            foreach (var user in Users)
            {
                yield return new ApplicationUser
                {
                    Id = user.IdentityId,
                    UserName = user.Username,
                    Email = GetEmail(user.Username),
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, GetPassword()),
                    NormalizedUserName = user.Username.ToUpper(),
                    NormalizedEmail = GetEmail(user.Username).ToUpper(),
                    SecurityStamp = string.Empty
                };
            }
        }
    }
}
