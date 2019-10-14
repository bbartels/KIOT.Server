using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using KIOT.Server.Dto.Application.Authentication;
using KIOT.Server.Api.Tests.Integration.Utility;
using Xunit.Abstractions;

namespace KIOT.Server.Api.Tests.Integration.Controller
{
    public class AccountsControllerIntegrationTests : IClassFixture<InMemoryKIOTApiFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IClient _client;
        private readonly InMemoryKIOTApiFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public AccountsControllerIntegrationTests(InMemoryKIOTApiFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _factory.SeedDbContext();
            _client = new Client("http://localhost:5000", _httpClient);
            _output = output;
        }

        [Fact]
        public async Task RegisterCustomer_ValidData_Succeeds()
        {
            var customer = new RegisterCustomerDto
            {
                CustomerCode = "0055_ALT-CM0102",
                Email = "iclive@fake.com",
                Username = "iclive",
                FirstName = "Ian",
                LastName = "Clive",
                Password = "password",
                PhoneNumber = "+4412345678",
            };

            Assert.Null(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.Null(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.Customers.Any(x => x.FirstName == customer.FirstName && x.LastName == customer.LastName
                                    && x.Username == customer.Username && x.PhoneNumber == customer.PhoneNumber));
            }

            using (var context = _factory.GetIdentityContext())
            {
                Assert.True(context.Users.Any(x => x.Email == customer.Email && x.UserName == customer.Username));
            }
        }

        [Fact]
        public async Task RegisterCustomer_ExistingCustomerCode_Fails()
        {
            var customer = new RegisterCustomerDto
            {
                CustomerCode = "0055_ALT-CM0103",
                Email = "iclive@fake.com",
                Username = "iclive",
                FirstName = "Ian",
                LastName = "Clive",
                Password = "password",
                PhoneNumber = "+4412345678",
            };

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.False(context.Customers.Any(x => x.Username == "iclive"));
            }

            using (var context = _factory.GetIdentityContext())
            {
                Assert.False(context.Users.Any(x => x.UserName == customer.Username));
            }
        }

        [Fact]
        public async Task RegisterCustomer_InvalidCustomerCode_Fails()
        {
            var customer = new RegisterCustomerDto
            {
                CustomerCode = "0000_ALT-AB0000",
                Email = "iclive@fake.com",
                Username = "iclive",
                FirstName = "Ian",
                LastName = "Clive",
                Password = "password",
                PhoneNumber = "+4412345678",
            };

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));
        }

        [Fact]
        public async Task RegisterCustomer_InvalidData_Fails()
        {
            var customer = new RegisterCustomerDto
            {
                CustomerCode = "0055_ALT-CM0102",
                Email = "iclive@fake",
                Username = "iclive",
                FirstName = "Ian",
                LastName = "Clive",
                Password = "password",
                PhoneNumber = "+4412345678",
            };

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            customer.Email = "iclive@fake.com";
            customer.FirstName = null;

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            customer.FirstName = "Ian";
            customer.PhoneNumber = "1234566";

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCustomerAsync(customer)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));
        }

        [Fact]
        public async Task RegisterCaretaker_InvalidData_Fails()
        {
            var caretaker = new RegisterCaretakerDto
            {
                Email = "iclive@fake",
                Username = "iclive",
                FirstName = "Ian",
                LastName = "Clive",
                Password = "password",
                PhoneNumber = "+4412345678",
            };

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCaretakerAsync(caretaker)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            caretaker.Email = "iclive@fake.com";
            caretaker.FirstName = null;

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCaretakerAsync(caretaker)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));

            caretaker.FirstName = "Ian";
            caretaker.PhoneNumber = "1234566";

            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCaretakerAsync(caretaker)));
            Assert.NotNull(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("iclive", "password")));
        }

        [Fact]
        public async Task RegisterCaretaker_ValidData_Succeeds()
        {
            var caretaker = new RegisterCaretakerDto
            {
                Email = "cthurman@fake.com",
                Username = "cthurman",
                FirstName = "Chris",
                LastName = "Thurman",
                Password = "password",
                PhoneNumber = "+4487654321",
            };

            Assert.Null(await Record.ExceptionAsync(async () => await _client.Accounts_RegisterCaretakerAsync(caretaker)));
            Assert.Null(await Record.ExceptionAsync(async () => await _client.AuthenticateUserAsync("cthurman", "password")));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.Caretakers.Any(x => x.FirstName == caretaker.FirstName && x.LastName == caretaker.LastName
                                    && x.Username == caretaker.Username && x.PhoneNumber == caretaker.PhoneNumber));
            }

            using (var context = _factory.GetIdentityContext())
            {
                Assert.True(context.Users.Any(x => x.Email == caretaker.Email && x.UserName == caretaker.Username));
            }
        }
    }
}
