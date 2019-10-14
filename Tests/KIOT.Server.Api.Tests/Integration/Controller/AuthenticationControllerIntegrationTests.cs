using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

using KIOT.Server.Dto.Application.Authentication;
using KIOT.Server.Api.Tests.Integration.Utility;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Customers.Caretakers;

namespace KIOT.Server.Api.Tests.Integration.Controller
{
    public class AuthenticationControllerIntegrationTests : IClassFixture<InMemoryKIOTApiFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IClient _client;
        private readonly InMemoryKIOTApiFactory<Startup> _factory;

        public AuthenticationControllerIntegrationTests(InMemoryKIOTApiFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _factory.SeedDbContext();
            _client = new Client("http://localhost:5000", _httpClient);
        }

        [Fact]
        public async Task AuthenticateUser_ValidCredentials_Succeeds()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            Assert.NotNull(dto);
            Assert.NotNull(dto.AccessToken);
            Assert.NotNull(dto.RefreshToken);
            Assert.Equal(900, dto.ExpiresIn);
        }

        [Fact]
        public async Task AuthenticateUser_ValidCredentials_AllowsAuthorizedAccess()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            Assert.NotNull(dto);
            _httpClient.AddAuthorization(dto);

            AssignedCaretakersForCustomerDto dto2 = null;
            var exception2 = await Record.ExceptionAsync(async () =>
            {
                dto2 = await _client.Customer_GetAssignedCaretakersAsync();
            });

            Assert.Null(exception2);
            Assert.NotNull(dto2);
        }

        [Fact]
        public async Task AuthenticateUser_ValidCredentials_ReturnsValidAccessToken()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            Assert.NotNull(dto);
            Assert.NotNull(dto.AccessToken);
            Assert.NotNull(dto.RefreshToken);
            dto.AssertValidToken();
        }

        [Fact]
        public async Task AuthenticateUser_InvalidPassword_Fails()
        {
            AccessTokenDto dto = null;
            var exception = await Record.ExceptionAsync(async () =>
            {
                dto = await _client.Authentication_AuthenticateAsync(new LoginUserDto { Username = "mwilson", Password = "invalidPassword" });
            });

            Assert.NotNull(exception);
            Assert.Null(dto);
        }

        [Fact]
        public async Task AuthenticateUser_InvalidUsername_Fails()
        {
            AccessTokenDto dto = null;
            var exception = await Record.ExceptionAsync(async () =>
            {
                dto = await _client.Authentication_AuthenticateAsync(new LoginUserDto { Username = "invalidUsername", Password = "password" });
            });

            Assert.NotNull(exception);
            Assert.Null(dto);
        }

        [Fact]
        public async Task ExchangeRefreshToken_ValidToken_Succeeds()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            Assert.NotNull(dto);

            AccessTokenDto dto2 = null;
            var exception2 = await Record.ExceptionAsync(async () =>
            {
                dto2 = await _client.Authentication_ExchangeRefreshTokenAsync(new ExchangeRefreshTokenDto
                {
                    AccessToken = dto.AccessToken,
                    RefreshToken = dto.RefreshToken
                });
            });

            Assert.Null(exception2);
            dto2.AssertValidToken();
        }

        [Fact]
        public async Task ExchangeRefreshToken_InvalidAccessToken_Fails()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            AccessTokenDto dto2 = null;
            var exception2 = await Record.ExceptionAsync(async () =>
            {
                dto2 = await _client.Authentication_ExchangeRefreshTokenAsync(new ExchangeRefreshTokenDto
                {
                    AccessToken = "InvalidToken",
                    RefreshToken = dto.RefreshToken
                });
            });

            Assert.NotNull(exception2);
        }

        [Fact]
        public async Task RegisterPushToken_ValidRequest_Succeeds()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            _httpClient.AddAuthorization(dto);

            var pushTokenDto = new RegisterPushTokenDto
            {
                PushToken = "4bb26a87-2ccb-48b7-ba66-c1025b22056e",
                DeviceName = "Test Device",
                InstallationId = "6eb6ef3e-e1b0-428f-832a-bbf6d36c9e9c",
                MobileOs = MobileOS.Android
            };

            SuccessfulRequestDto dto2 = null;
            var exception2 = await Record.ExceptionAsync(async () =>
            {
                dto2 = await _client.Authentication_RegisterPushTokenAsync(pushTokenDto);
            });

            Assert.Null(exception2);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.PushTokens.Include(x => x.MobileDevice).Any(x => x.MobileDevice.DeviceName == pushTokenDto.DeviceName
                                                 && x.Token == pushTokenDto.PushToken && x.MobileDevice.InstallationId ==
                                                 pushTokenDto.InstallationId && x.MobileDevice.MobileOS == Core.Models.Application.MobileOS.Android));
            }
        }

        [Fact]
        public async Task RegisterPushToken_InvalidRequest_Fails()
        {
            var dto = await _client.AuthenticateUserAsync("mwilson", "password");

            _httpClient.AddAuthorization(dto);

            var pushTokenDto = new RegisterPushTokenDto
            {
                PushToken = null,
                DeviceName = "Test Device",
                InstallationId = "6eb6ef3e-e1b0-428f-832a-bbf6d36c9e9c",
                MobileOs = MobileOS.Android
            };

            SuccessfulRequestDto dto2 = null;
            var exception2 = await Record.ExceptionAsync(async () =>
            {
                dto2 = await _client.Authentication_RegisterPushTokenAsync(pushTokenDto);
            });

            using (var context = _factory.GetKIOTContext())
            {
                Assert.False(context.PushTokens.Include(x => x.MobileDevice).Any(x => x.MobileDevice.DeviceName == pushTokenDto.DeviceName
                                                 && x.Token == pushTokenDto.PushToken && x.MobileDevice.InstallationId ==
                                                 pushTokenDto.InstallationId && x.MobileDevice.MobileOS == Core.Models.Application.MobileOS.Android));
            }

            Assert.NotNull(exception2);
        }
    }
}
