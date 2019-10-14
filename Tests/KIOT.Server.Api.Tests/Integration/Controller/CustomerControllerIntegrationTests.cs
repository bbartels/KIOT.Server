using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using KIOT.Server.Api.Tests.Integration.Utility;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Caretakers;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Application.Customers.Tasks;

namespace KIOT.Server.Api.Tests.Integration.Controller
{
    public class CustomerControllerIntegrationTests : IClassFixture<InMemoryKIOTApiFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IClient _client;
        private readonly InMemoryKIOTApiFactory<Startup> _factory;

        public CustomerControllerIntegrationTests(InMemoryKIOTApiFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _factory.SeedDbContext();
            _client = new Client("http://localhost:5000", _httpClient);
        }

        [Fact]
        public async Task AddCaretakerForCustomer_ValidRequest_Successful()
        {
            (await _client.AuthenticateUserAsync("fbrown", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_AddCaretakerForCustomerAsync(
                    new AddCaretakerForCustomerDto {CaretakerUsername = "jcole"})));
            Assert.NotNull(response);


            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            PendingCaretakerRequestsDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response1);
            Assert.Single(response1.CaretakerRequests);
            Assert.Contains(response1.CaretakerRequests, x =>
                x.Customer.FirstName == "Frank" && x.Customer.LastName == "Brown" &&
                x.Customer.Username == "fbrown" && x.Customer.PhoneNumber == "+4418821001");
        }

        [Fact]
        public async Task AddCaretaker_ExistingRequest_Fails()
        {
            (await _client.AuthenticateUserAsync("fbrown", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_AddCaretakerForCustomerAsync(
                    new AddCaretakerForCustomerDto {CaretakerUsername = "twilliams"})));
            Assert.Null(response);


            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            PendingCaretakerRequestsDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response1);
            Assert.Empty(response1.CaretakerRequests);
        }

        [Fact]
        public async Task AddCaretakerForCustomer_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () => _client.Customer_AddCaretakerForCustomerAsync(null));

        [Fact]
        public async Task AddCaretakerForCustomer_InvalidUsername_Fails()
        {
            (await _client.AuthenticateUserAsync("fbrown", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_AddCaretakerForCustomerAsync(
                    new AddCaretakerForCustomerDto {CaretakerUsername = "invalidUsername"})));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCustomerHomepage_ValidRequest_Successful()
        {
            (await _client.AuthenticateUserAsync("fbrown", "password")).AddAuthorization(_httpClient);

            CustomerHomepageDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_GetHomepageAsync()));
            Assert.NotNull(response);
            Assert.NotNull(response.Customer);
            Assert.True(response.Customer.FirstName == "Frank" && response.Customer.LastName == "Brown"
                                                               && response.Customer.Username == "fbrown");
            Assert.Equal(3, response.PowerUsageRatios.Count());
            Assert.Equal(3, response.PowerUsageRatios.Select(x => x.TimePeriod).Distinct().Count());
        }

        [Fact]
        public async Task GetCustomerHomepage_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, _client.Customer_GetHomepageAsync);

        [Fact]
        public async Task GetAssignedCaretakers_ValidRequest_Successful()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            AssignedCaretakersForCustomerDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_GetAssignedCaretakersAsync()));
            Assert.NotNull(response);
            Assert.Empty(response.Caretakers);

            using (var context = _factory.GetKIOTContext())
            {
                var caretaker = context.Caretakers.SingleOrDefault(x => x.Username == "jcole");
                Assert.NotNull(caretaker);
                var customer = context.Customers.SingleOrDefault(x => x.Username == "mwilson");
                Assert.NotNull(customer);
                context.IsCaredForBys.Add(new CaretakerForCustomer(caretaker.Id, customer.Id));
                context.SaveChanges();
            }

            response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_GetAssignedCaretakersAsync()));
            Assert.NotNull(response);
            Assert.Single(response.Caretakers);
        }

        [Fact]
        public async Task GetAssignedCaretakers_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, _client.Customer_GetAssignedCaretakersAsync);

        [Fact]
        public async Task DeleteAssignedCaretaker_ValidRequest_Successful()
        {
            using (var context = _factory.GetKIOTContext())
            {
                var caretaker = context.Caretakers.SingleOrDefault(x => x.Username == "jcole");
                Assert.NotNull(caretaker);
                var customer = context.Customers.SingleOrDefault(x => x.Username == "mwilson");
                Assert.NotNull(customer);
                context.IsCaredForBys.Add(new CaretakerForCustomer(caretaker.Id, customer.Id));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_DeleteAssignedCaretakerAsync(Guid.Parse("e0e26424-f4cd-4400-bc78-28b0bfef729a"))));
            Assert.NotNull(response);

            AssignedCaretakersForCustomerDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Customer_GetAssignedCaretakersAsync()));
            Assert.NotNull(response1);
            Assert.Empty(response1.Caretakers);
        }

        [Fact]
        public async Task DeleteAssignedCaretaker_InvalidCaretaker_Successful()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_DeleteAssignedCaretakerAsync(Guid.Parse("e0e26424-f4cd-4400-bc78-28b0bfef729a"))));
            Assert.Null(response);

            AssignedCaretakersForCustomerDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Customer_GetAssignedCaretakersAsync()));
            Assert.NotNull(response1);
            Assert.Empty(response1.Caretakers);
        }

        [Fact]
        public async Task DeleteAssignedCaretaker_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Customer_DeleteAssignedCaretakerAsync(Guid.Parse("e0e26424-f4cd-4400-bc78-28b0bfef729a")));
        }
        
        public async Task GetDetailedPage_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            var list = new List<Task<CustomerDetailedPageDto>>();
            Assert.Null(await Record.ExceptionAsync(async () =>
            {
                list.Add(_client.Customer_GetDetailedPageAsync(TimeInterval.Day, 0));
                list.Add(_client.Customer_GetDetailedPageAsync(TimeInterval.Week, 0));
                list.Add(_client.Customer_GetDetailedPageAsync(TimeInterval.Month, 0));
                list.Add(_client.Customer_GetDetailedPageAsync(TimeInterval.Year, 0));
                list.Add(_client.Customer_GetDetailedPageAsync(TimeInterval.Day, 1));
                await Task.WhenAll(list);
            }));

            foreach (var dto in list.Select(x => x.Result))
            {
                Assert.NotNull(dto);
                Assert.NotNull(dto.Appliances);
                Assert.NotEqual(0, dto.DataPointEvery);
                Assert.InRange(dto.StartTimeInterval, dto.StartTime, dto.EndTime);
                Assert.InRange(dto.EndTimeInterval, dto.StartTime, dto.EndTime);
            }
        }

        [Fact]
        public async Task GetDetailedPage_InvalidRequest_Fails()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            CustomerDetailedPageDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_GetDetailedPageAsync(TimeInterval.Day, -1)));
            Assert.Null(response);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_GetDetailedPageAsync((TimeInterval)12, -1)));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetDetailedPage_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>_client.Customer_GetDetailedPageAsync(TimeInterval.Day, 0));

        public async Task SetAliasForAppliance_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = "ApplianceAlias", ApplianceId = 20501 })));
            Assert.NotNull(response);

            CustomerAppliancesDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response1 = await _client.Customer_GetAppliancesAsync()));
            Assert.NotNull(response1);
            Assert.Contains(response1.Appliances, x => x.ApplianceId == 20501 && x.ApplianceName == "ApplianceAlias");
        }

        public async Task SetAliasForAppliance_InvalidApplianceId_Fails()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = "ApplianceAlias", ApplianceId = 0 })));
            Assert.Null(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.False(context.CustomerAppliances
                    .Any(x => x.Customer.Username == "mwilson" && x.Alias == "ApplianceAlias"));
            }
        }

        public async Task SetAliasForAppliance_MultipleAssignment_ReturnsLatestAssignedAlias()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = "ApplianceAlias1", ApplianceId = 20501 })));
            Assert.NotNull(response);

            SuccessfulRequestDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = "ApplianceAlias2", ApplianceId = 20501 })));
            Assert.NotNull(response1);

            CustomerAppliancesDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Customer_GetAppliancesAsync()));
            Assert.NotNull(response2);
            Assert.Contains(response2.Appliances, x => x.ApplianceName == "ApplianceAlias2" && x.ApplianceId == 20501);
        }

        public async Task SetAliasForAppliance_ClearAlias_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = "ApplianceAlias", ApplianceId = 20501 })));
            Assert.NotNull(response);

            SuccessfulRequestDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Customer_SetAliasForApplianceAsync(
                    new SetAliasForApplianceDto { Alias = null, ApplianceId = 20501 })));
            Assert.NotNull(response1);

            CustomerAppliancesDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Customer_GetAppliancesAsync()));
            Assert.NotNull(response2);
            Assert.Contains(response2.Appliances, x => x.ApplianceName == null && x.ApplianceId == 20501);
        }

        [Fact]
        public async Task SetAliasForAppliance_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Customer_SetAliasForApplianceAsync(new SetAliasForApplianceDto { Alias = "Test", ApplianceId = 123 } ));
        }

        public async Task GetAppliances_ValidRequest_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                var type = context.ApplianceTypes.SingleOrDefault(x => x.ApplianceTypeId == 1);
                if (type == null)
                {
                    type = new ApplianceType(1, "TestAppliance");
                    context.ApplianceTypes.Add(type);
                    context.SaveChanges();
                }
                context.CustomerAppliances.Add(new CustomerAppliance(20501, "TestAlias", -1, type));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            CustomerAppliancesDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () => response = await _client.Customer_GetAppliancesAsync()));
            Assert.NotNull(response);
            Assert.NotEmpty(response.Appliances);
            Assert.Contains(response.Appliances, x => x.ApplianceId == 20501 && x.ApplianceName == "TestAlias");
        }

        [Fact]
        public async Task GetAppliances_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () => _client.Customer_GetAppliancesAsync());

        [Fact]
        public async Task GetAssignedTasks_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            CustomerTasksDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () => response = await _client.Customer_GetAssignedTasksAsync(TaskOption.Both)));
            Assert.NotNull(response);
            Assert.Empty(response.Tasks);

            using (var context = _factory.GetKIOTContext())
            {
                var caretaker = context.Caretakers.SingleOrDefault(x => x.Username == "jcole");
                Assert.NotNull(caretaker);
                var customer = context.Customers.SingleOrDefault(x => x.Username == "mwilson");
                Assert.NotNull(customer);
                context.CustomerTasks.Add(new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", DateTime.UtcNow.AddDays(1)));
                context.SaveChanges();
            }

            response = null;
            Assert.Null(await Record.ExceptionAsync(async () => response = await _client.Customer_GetAssignedTasksAsync(TaskOption.Both)));
            Assert.NotNull(response);
            Assert.Single(response.Tasks);
            var task = response.Tasks.First();

            CustomerTasksDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response1 = await _client.Customer_GetAssignedTasksAsync(TaskOption.Finished)));
            Assert.NotNull(response1);
            Assert.Empty(response1.Tasks);

            CustomerTasksDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Customer_GetAssignedTasksAsync(TaskOption.Unfinished)));
            Assert.NotNull(response2);
            Assert.Single(response2.Tasks);

            Assert.Equal("TestTitle", task.Title);
            Assert.Equal("TestDescription", task.Description);
            Assert.InRange(DateTime.UtcNow, task.StartedAt, task.ExpiresAt ?? DateTime.UtcNow);
            Assert.Equal("jcole", task.AssignedBy);
            Assert.False(task.Finished);
        }

        [Fact]
        public async Task GetAssignedTasks_InvalidTaskOption_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            CustomerTasksDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () => response = await _client.Customer_GetAssignedTasksAsync((TaskOption)123)));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetAssignedTasks_NoAuthorization_Fails() =>
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () => _client.Customer_GetAssignedTasksAsync(TaskOption.Both));
    }
}
