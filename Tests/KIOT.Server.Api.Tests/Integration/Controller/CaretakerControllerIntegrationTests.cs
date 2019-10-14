using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

using KIOT.Server.Api.Tests.Integration.Utility;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Caretakers.Data;
using KIOT.Server.Dto.Application.Caretakers.Tasks;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Api.Tests.Integration.Controller
{
    public class CaretakerControllerIntegrationTests : IClassFixture<InMemoryKIOTApiFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IClient _client;
        private readonly InMemoryKIOTApiFactory<Startup> _factory;

        public CaretakerControllerIntegrationTests(InMemoryKIOTApiFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _factory.SeedDbContext();
            _client = new Client("http://localhost:5000", _httpClient);
        }

        [Fact]
        public async Task GetPendingCaretakerRequests_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("twilliams", "password")).AddAuthorization(_httpClient);

            PendingCaretakerRequestsDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () => response = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response);
            Assert.Equal(3, response.CaretakerRequests.Count());

            foreach (var request in response.CaretakerRequests)
            {
                Assert.NotNull(request);
                Assert.NotNull(request.Customer);
                Assert.NotNull(request.Customer.FirstName);
                Assert.NotNull(request.Customer.LastName);
                Assert.NotNull(request.Customer.Username);
                Assert.InRange(request.Timestamp, request.Timestamp, DateTime.UtcNow);
            }
        }


        [Fact]
        public async Task GetPendingCaretakerRequests_NoAuthorization_Fails() =>
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, _client.Caretaker_GetPendingCaretakerRequestsAsync);


        [Fact]
        public async Task HandleCaretakerRequest_AcceptRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("twilliams", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_HandleCaretakerRequestAsync(Guid.Parse("f61ee92d-792e-45dd-a664-309178a71830"),
                    new HandleCaretakerForCustomerRequestDto { AcceptRequest = true})));
            Assert.NotNull(response);

            PendingCaretakerRequestsDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response1 = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response1);
            Assert.Equal(2, response1.CaretakerRequests.Count());

            AssignedCustomersForCaretakerDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Caretaker_GetAssignedCustomersAsync()));
            Assert.NotNull(response2);
            Assert.Single(response2.Customers);
        }

        [Fact]
        public async Task HandleCaretakerRequest_DeclineRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("twilliams", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_HandleCaretakerRequestAsync(Guid.Parse("f61ee92d-792e-45dd-a664-309178a71830"),
                    new HandleCaretakerForCustomerRequestDto { AcceptRequest = false})));
            Assert.NotNull(response);

            PendingCaretakerRequestsDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response1 = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response1);
            Assert.Equal(2, response1.CaretakerRequests.Count());

            AssignedCustomersForCaretakerDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Caretaker_GetAssignedCustomersAsync()));
            Assert.NotNull(response2);
            Assert.Empty(response2.Customers);
        }

        [Fact]
        public async Task HandleCaretakerRequest_InvalidRequest_Fails()
        {
            (await _client.AuthenticateUserAsync("twilliams", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_HandleCaretakerRequestAsync(Guid.Parse("f61ee92d-792e-45dd-a664-000000000000"),
                    new HandleCaretakerForCustomerRequestDto { AcceptRequest = true})));
            Assert.Null(response);

            PendingCaretakerRequestsDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response1 = await _client.Caretaker_GetPendingCaretakerRequestsAsync()));
            Assert.NotNull(response1);
            Assert.Equal(3, response1.CaretakerRequests.Count());

            AssignedCustomersForCaretakerDto response2 = null;
            Assert.Null(await Record.ExceptionAsync(async () => response2 = await _client.Caretaker_GetAssignedCustomersAsync()));
            Assert.NotNull(response2);
            Assert.Empty(response2.Customers);
        }

        [Fact]
        public async Task HandleCaretakerRequest_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient,
                () => _client.Caretaker_HandleCaretakerRequestAsync(Guid.Empty,
                    new HandleCaretakerForCustomerRequestDto { AcceptRequest = true }));
        }

        [Fact]
        public async Task GetAssignedCustomers_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("twilliams", "password")).AddAuthorization(_httpClient);

            AssignedCustomersForCaretakerDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetAssignedCustomersAsync()));
            Assert.NotNull(response);
            Assert.Empty(response.Customers);

            SuccessfulRequestDto response1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response1 = await _client.Caretaker_HandleCaretakerRequestAsync(Guid.Parse("f61ee92d-792e-45dd-a664-309178a71830"),
                    new HandleCaretakerForCustomerRequestDto { AcceptRequest = true} )));
            Assert.NotNull(response1);

            response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetAssignedCustomersAsync()));
            Assert.NotNull(response);
            Assert.Single(response.Customers);
            var customer = response.Customers.First();
            Assert.NotNull(customer);
            Assert.Equal("Matt", customer.FirstName);
            Assert.Equal("Wilson", customer.LastName);
            Assert.Equal("mwilson", customer.Username);
            Assert.Equal("+4418821000", customer.PhoneNumber);
        }

        [Fact]
        public async Task GetAssignedCustomers_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () => _client.Caretaker_GetAssignedCustomersAsync());
        }

        [Fact]
        public async Task GetCustomerHomepage_ValidRequest_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "fbrown");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            CustomerHomepageForCaretakerDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetCustomerHomepageAsync(Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"))));
            Assert.NotNull(response);
            Assert.NotNull(response.Customer);
            Assert.True(response.Customer.FirstName == "Frank" && response.Customer.LastName == "Brown"
                                                               && response.Customer.Username == "fbrown");
            Assert.Equal(3, response.PowerUsageRatios.Count());
            Assert.Equal(3, response.PowerUsageRatios.Select(x => x.TimePeriod).Distinct().Count());
        }

        [Fact]
        public async Task GetCustomerHomepage_InvalidCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            CustomerHomepageForCaretakerDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetCustomerHomepageAsync(Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"))));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCustomerHomepage_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_GetCustomerHomepageAsync(Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2")));
        }

        [Fact]
        public async Task DeleteAssignedCustomer_ValidRequest_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "fbrown");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_DeleteAssignedCustomerAsync(Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"))));
            Assert.NotNull(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.False(context.IsCaredForBys.Include($"{nameof(CaretakerForCustomer.Caretaker)}")
                    .Include($"{nameof(CaretakerForCustomer.Customer)}")
                    .Any(x => x.Caretaker.Username == "jcole" && x.Customer.Username == "fbrown"));
            }
        }

        [Fact]
        public async Task DeleteAssignedCustomer_InvalidCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_DeleteAssignedCustomerAsync(Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"))));
            Assert.Null(response);
        }

        [Fact]
        public async Task DeleteAssignedCustomer_EmptyGuid_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_DeleteAssignedCustomerAsync(Guid.Empty)));
            Assert.Null(response);
        }

        [Fact]
        public async Task DeleteAssignedCustomer_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_DeleteAssignedCustomerAsync(Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2")));
        }

        [Fact]
        public async Task AlertCustomer_NonAssignedCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_AlertCustomerAsync(new AlertCustomerDto { Username = "mwilson", Message = "TestMessage" })));
            Assert.Null(response);
        }

        [Fact]
        public async Task AlertCustomer_InvalidRequest_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_AlertCustomerAsync(new AlertCustomerDto { Username = "mwilson", Message = null })));
            Assert.Null(response);
        }

        [Fact]
        public async Task AlertCustomer_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_AlertCustomerAsync(new AlertCustomerDto() { Username = "mwilson", Message = "TestMessage" }));
        }

        [Fact]
        public async Task RegisterCustomerTask_ValidRequest_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "fbrown");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto
                {
                    CustomerGuid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"),
                    Title = "TestTitle",
                    Description = "TaskDescription",
                    ExpiresAt = null,
                })));
            Assert.NotNull(response);

            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto
                {
                    CustomerGuid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"),
                    Title = "TestTitle",
                    Description = "TaskDescription",
                    ExpiresAt = DateTime.UtcNow.AddDays(2),
                })));
            Assert.NotNull(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(2, context.CustomerTasks.Count(x =>
                    x.Customer.Guid == Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557") && x.Caretaker.Username == "jcole"));
            }
        }

        [Fact]
        public async Task RegisterCustomerTask_NonAssignedCustomers_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto
                {
                    CustomerGuid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"),
                    Title = "TestTitle",
                    Description = "TaskDescription",
                    ExpiresAt = null,
                })));
            Assert.Null(response);
        }

        [Fact]
        public async Task RegisterCustomerTask_InvalidRequest_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto
                {
                    CustomerGuid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"),
                    Title = null,
                    Description = "TaskDescription",
                    ExpiresAt = null,
                })));
            Assert.Null(response);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto
                {
                    CustomerGuid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"),
                    Title = "TestTitle",
                    Description = null,
                    ExpiresAt = null,
                })));
            Assert.Null(response);
        }

        [Fact]
        public async Task RegisterCustomerTask_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_RegisterCustomerTaskAsync(new RegisterCustomerTaskDto()));
        }

        [Fact]
        public async Task SetCustomerTaskState_CompleteTask_Succeeds()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var (customer, caretaker) = context.AddCaretakerForCustomer("jcole", "fbrown");
                var task = new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", null);
                guid = task.Guid;
                Assert.False(task.TaskFinished);
                context.CustomerTasks.Add(task);
                context.SaveChanges();
            }

            Assert.NotEqual(Guid.Empty, guid);

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto
                {
                    CustomerTaskGuid = guid,
                    State = CustomerTaskState.Complete
                })));
            Assert.NotNull(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.CustomerTasks.SingleOrDefault(x => x.Guid == guid)?.TaskFinished ?? false);
            }
        }

        [Fact]
        public async Task SetCustomerTaskState_CancelTask_Succeeds()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var (customer, caretaker) = context.AddCaretakerForCustomer("jcole", "fbrown");
                var task = new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", null);
                guid = task.Guid;
                Assert.False(task.TaskFinished);
                context.CustomerTasks.Add(task);
                context.SaveChanges();
            }

            Assert.NotEqual(Guid.Empty, guid);

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto
                {
                    CustomerTaskGuid = guid,
                    State = CustomerTaskState.Cancel
                })));
            Assert.NotNull(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Null(context.CustomerTasks.SingleOrDefault(x => x.Guid == guid));
            }
        }

        [Fact]
        public async Task SetCustomerTaskState_CancelTaskWhenFinished_Fails()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var (customer, caretaker) = context.AddCaretakerForCustomer("jcole", "fbrown");
                var task = new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", null);
                guid = task.Guid;
                task.CompleteTask();
                Assert.True(task.TaskFinished);
                context.CustomerTasks.Add(task);
                context.SaveChanges();
            }

            Assert.NotEqual(Guid.Empty, guid);

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto
                {
                    CustomerTaskGuid = guid,
                    State = CustomerTaskState.Cancel
                })));
            Assert.Null(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.CustomerTasks.Any(x => x.Guid == guid));
            }
        }

        [Fact]
        public async Task SetCustomerTaskState_InvalidTaskGuid_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto
                {
                    CustomerTaskGuid = Guid.Parse("00000000-2c48-4f8b-a318-000000000000"),
                    State = CustomerTaskState.Complete
                })));
            Assert.Null(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(0, context.CustomerTasks.Count(x => x.Caretaker.Username == "jcole"));
            }
        }

        [Fact]
        public async Task SetCustomerTaskState_InvalidTaskState_Fails()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var (customer, caretaker) = context.AddCaretakerForCustomer("jcole", "fbrown");
                var task = new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", null);
                guid = task.Guid;
                Assert.False(task.TaskFinished);
                context.CustomerTasks.Add(task);
                context.SaveChanges();
            }

            Assert.NotEqual(Guid.Empty, guid);

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto
                {
                    CustomerTaskGuid = guid,
                    State = (CustomerTaskState) 12
                })));
            Assert.Null(response);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(1, context.CustomerTasks.Count(x => x.Caretaker.Username == "jcole"));
            }
        }

        [Fact]
        public async Task SetCustomerTaskState_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_SetCustomerTaskStateAsync(new SetCustomerTaskStateDto()));
        }

        [Fact]
        public async Task GetCustomerTasks_NoRegisteredTasks_ReturnsEmptyCollection()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "mwilson");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            CustomerTasksForCaretakerDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetCustomerTasksAsync(Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2"))));
            Assert.NotNull(response);
            Assert.Empty(response.Tasks);
            Assert.NotNull(response.Customer);
            Assert.Equal("mwilson", response.Customer.Username);
            Assert.Equal("Matt", response.Customer.FirstName);
            Assert.Equal("Wilson", response.Customer.LastName);
            Assert.Equal("+4418821000", response.Customer.PhoneNumber);
        }

        [Fact]
        public async Task GetCustomerTasks_ExistingTask_ReturnsNonEmptyCollection()
        {
            using (var context = _factory.GetKIOTContext())
            {
                var (customer, caretaker) = context.AddCaretakerForCustomer("jcole", "mwilson");
                var task = new CustomerTask(customer, caretaker, "TestDescription", "TestTitle", null);
                Assert.False(task.TaskFinished);
                context.CustomerTasks.Add(task);
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            CustomerTasksForCaretakerDto response = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetCustomerTasksAsync(Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2"))));
            Assert.NotNull(response);
            Assert.Single(response.Tasks);
            var responseTask = response.Tasks.First();
            Assert.Equal("TestTitle", responseTask.Title);
            Assert.Equal("TestDescription", responseTask.Description);
            Assert.False(responseTask.Finished);
        }

        [Fact]
        public async Task GetCustomerTasks_NonAssignedCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            CustomerTasksForCaretakerDto response = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                response = await _client.Caretaker_GetCustomerTasksAsync(Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2"))));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetCustomerTasks_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_GetCustomerTasksAsync(Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557")));
        }

        public async Task GetCustomerApplianceHistory_ValidRequest_ReturnsValidHistory()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "fbrown");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            var guid = Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557");

            var list = new List<Task<CustomerDetailedPageDto>>();
            Assert.Null(await Record.ExceptionAsync(async () =>
            {
                list.Add(_client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Day, 0));
                list.Add(_client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Week, 0));
                list.Add(_client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Year, 0));
                list.Add(_client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Day, 1));
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
        public async Task GetCustomerApplianceHistory_InvalidParameters_Fails()
        {
            using (var context = _factory.GetKIOTContext())
            {
                _ = context.AddCaretakerForCustomer("jcole", "mwilson");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            var guid = Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2");

            CustomerDetailedPageDto dto = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
            {
                dto = await _client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Day, -1);
            }));
            Assert.Null(dto);
        }

        [Fact]
        public async Task GetCustomerApplianceHistory_NonAssignedCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            var guid = Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2");

            CustomerDetailedPageDto dto = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
            {
                dto = await _client.Caretaker_GetCustomerApplianceHistoryAsync(guid, TimeInterval.Day, 0);
            }));
            Assert.Null(dto);
        }

        [Fact]
        public async Task GetCustomerApplianceHistory_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Caretaker_GetCustomerApplianceHistoryAsync(
                    Guid.Parse("48dd8db1-2c48-4f8b-a318-8a85cc286557"), TimeInterval.Day, 0));
        }
    }
}
