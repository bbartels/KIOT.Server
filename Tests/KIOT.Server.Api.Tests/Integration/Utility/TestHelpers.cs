using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Data.Persistence;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api.Tests.Integration.Utility
{
    internal static class TestExtensions
    {
        public static async Task<AccessTokenDto> AuthenticateUserAsync(this IClient client, string username, string password)
        {
            AccessTokenDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await client.Authentication_AuthenticateAsync(new LoginUserDto { Username = username, Password = password })));

            return dto;
        }

        public static void AssertValidToken(this AccessTokenDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(dto.AccessToken));
            var token = tokenHandler.ReadJwtToken(dto.AccessToken);

            Assert.Contains(token.Claims, claim => claim.Type == "iss" && claim.Value == "KIOT");
            Assert.Contains(token.Claims, claim => claim.Type == "role" && claim.Value == "Customer");
            Assert.Contains(token.Claims, claim => claim.Type == "given_name" && claim.Value == "Matt");
            Assert.Contains(token.Claims, claim => claim.Type == "family_name" && claim.Value == "Wilson");
            Assert.Contains(token.Claims, claim => claim.Type == "guid" && claim.Value == "d53a5412-efdd-4905-9f5d-5c2a624726a2");
            Assert.Contains(token.Claims, claim => claim.Type == "identity_id" && claim.Value == "24cfabf2-30e5-4e59-9be1-88dd861dec3c");
        }

        public static async Task AssertRequiresCustomerAuthorization(this IClient client, HttpClient httpClient,
            Func<Task> action) =>
                await client.AssertRequiresValidAuthorization(httpClient, "twilliams", "password", action);

        public static async Task AssertRequiresCaretakerAuthorization(this IClient client, HttpClient httpClient,
            Func<Task> action) =>
                await client.AssertRequiresValidAuthorization(httpClient, "mwilson", "password", action);

        public static async Task AssertRequiresValidAuthorization(this IClient client, HttpClient httpClient, string username, string password, Func<Task> action)
        {
            Assert.NotNull(await Record.ExceptionAsync(async () => await action()));

            AccessTokenDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await client.Authentication_AuthenticateAsync(new LoginUserDto { Username = username, Password = password })));

            dto.AddAuthorization(httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () => await action()));
        }

        public static (Customer, Caretaker) AddCaretakerForCustomer(this KIOTContext context, string caretakerUsername, string customerUsername)
        {
            var caretaker = context.Caretakers.SingleOrDefault(x => x.Username == caretakerUsername);
            Assert.NotNull(caretaker);
            var customer = context.Customers.SingleOrDefault(x => x.Username == customerUsername);
            Assert.NotNull(customer);

            context.IsCaredForBys.Add(new CaretakerForCustomer(caretaker.Id, customer.Id));
            context.SaveChanges();

            Assert.True(context.IsCaredForBys.Include($"{nameof(CaretakerForCustomer.Caretaker)}")
                .Include($"{nameof(CaretakerForCustomer.Customer)}")
                .Any(x => x.Caretaker.Username == caretakerUsername && x.Customer.Username == customerUsername));
            return (customer, caretaker);
        }
    }
}
