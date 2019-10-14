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
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Api.Tests.Integration.Controller
{
    public class ApplianceControllerIntegrationTests : IClassFixture<InMemoryKIOTApiFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IClient _client;
        private readonly InMemoryKIOTApiFactory<Startup> _factory;

        public ApplianceControllerIntegrationTests(InMemoryKIOTApiFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _factory.SeedDbContext();
            _client = new Client("http://localhost:5000", _httpClient);
        }

        [Fact]
        public async Task RegisterApplianceCategory_NewCategory_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.Null(await Record.ExceptionAsync(async () =>
                Assert.NotNull(await _client.Appliance_RegisterApplianceCategoryAsync("TestCategory"))));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.ApplianceCategories.Any(x => x.CustomerId == -1 && x.Name == "TestCategory"));
            }
        }

        [Fact]
        public async Task RegisterApplianceCategory_CategoryExists_Fails()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory", -1));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                Assert.Null(await _client.Appliance_RegisterApplianceCategoryAsync("TestCategory"))));
        }

        [Fact]
        public async Task RegisterApplianceCategory_CategoryNull_Fails()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                Assert.Null(await _client.Appliance_RegisterApplianceCategoryAsync(null))));
        }

        [Fact]
        public async Task RegisterApplianceCategory_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Appliance_RegisterApplianceCategoryAsync("TestCategory"));
        }

        [Fact]
        public async Task DeleteApplianceCategory_CategoryExists_Succeeds()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var category = new ApplianceCategory("TestCategory", -1);
                guid = category.Guid;
                context.ApplianceCategories.Add(category);
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.Null(await Record.ExceptionAsync(async () =>
                Assert.NotNull(await _client.Appliance_DeleteApplianceCategoryAsync(guid))));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(0, context.ApplianceCategories.Count(x => x.CustomerId == -1));
            }
        }

        [Fact]
        public async Task DeleteApplianceCategory_CategoryDoesNotExist_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                Assert.Null(await _client.Appliance_DeleteApplianceCategoryAsync(Guid.NewGuid()))));
        }

        [Fact]
        public async Task DeleteApplianceCategory_NonAssignedCategory_Fails()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var category = new ApplianceCategory("TestCategory", -1);
                guid = category.Guid;
                Assert.NotEqual(Guid.Empty, guid);
                context.ApplianceCategories.Add(category);
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("fbrown", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto dto = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_DeleteApplianceCategoryAsync(guid)));
            Assert.Null(dto);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(1, context.ApplianceCategories.Count(x => x.Guid == guid));
            }
        }

        public async Task DeleteApplianceCategory_CategoryHasAppliances_Succeeds()
        {
            Guid guid;
            using (var context = _factory.GetKIOTContext())
            {
                var category = new ApplianceCategory("TestCategory", -1);
                guid = category.Guid;
                Assert.NotEqual(Guid.Empty, guid);
                context.ApplianceCategories.Add(category);
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto dto1 = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto1 = await _client.Appliance_SetApplianceCategoryAsync(20501, "TestCategory")));
            Assert.NotNull(dto1);

            SuccessfulRequestDto dto2 = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                dto2 = await _client.Appliance_DeleteApplianceCategoryAsync(guid)));
            Assert.Null(dto2);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.Equal(1, context.ApplianceCategories.Count(x => x.Guid == guid));
            }
        }

        [Fact]
        public async Task DeleteApplianceCategory_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Appliance_DeleteApplianceCategoryAsync(Guid.NewGuid()));
        }

        public async Task SetApplianceCategory_ApplianceAndCategoryExists_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory", -1));
                context.SaveChanges();
            }
            
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_SetApplianceCategoryAsync(20501, "TestCategory")));
            Assert.NotNull(dto);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.Customers.Include(x => x.Appliances)
                    .ThenInclude(x => x.Category).SingleOrDefault(x => x.Username == "mwilson")?
                        .Appliances.Any(x => x.ApplianceId == 20501 && x.Category.Name == "TestCategory") ?? false);
            }
        }

        public async Task SetApplianceCategory_InvalidAppliance_Fails()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory", -1));
                context.SaveChanges();
            }
            
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                Assert.Null(await _client.Appliance_SetApplianceCategoryAsync(0, "TestCategory"))));

            using (var context = _factory.GetKIOTContext())
            {
                Assert.False(context.Customers.Include(x => x.Appliances)
                    .ThenInclude(x => x.Category).SingleOrDefault(x => x.Username == "mwilson")?
                        .Appliances.Any(x =>x.Category.Name == "TestCategory") ?? false);
            }
        }

        [Fact]
        public async Task SetApplianceCategory_InvalidCategory_Fails()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            Assert.NotNull(await Record.ExceptionAsync(async () =>
                Assert.Null(await _client.Appliance_SetApplianceCategoryAsync(20501, "InvalidCategory"))));
        }

        public async Task SetApplianceCategory_ClearCategory_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory", -1));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_SetApplianceCategoryAsync(20501, "TestCategory")));
            Assert.NotNull(dto);

            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_SetApplianceCategoryAsync(20501, null)));
            Assert.NotNull(dto);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.Customers.Include(x => x.Appliances)
                    .ThenInclude(x => x.Category).SingleOrDefault(x => x.Username == "mwilson")?
                        .Appliances.Any(x => x.ApplianceId == 20501 && x.Category == null) ?? false);
            }
        }

        public async Task SetApplianceCategory_UpdateCategory_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory1", -1));
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory2", -1));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            SuccessfulRequestDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_SetApplianceCategoryAsync(20501, "TestCategory1")));
            Assert.NotNull(dto);

            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_SetApplianceCategoryAsync(20501, "TestCategory2")));
            Assert.NotNull(dto);

            using (var context = _factory.GetKIOTContext())
            {
                Assert.True(context.Customers.Include(x => x.Appliances)
                    .ThenInclude(x => x.Category).SingleOrDefault(x => x.Username == "mwilson")?
                        .Appliances.Any(x => x.ApplianceId == 20501 && x.Category.Name == "TestCategory2") ?? false);
            }
        }

        [Fact]
        public async Task SetApplianceCategory_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Appliance_SetApplianceCategoryAsync(0, null));
        }

        [Fact]
        public async Task GetApplianceCategories_NoAssignedCategories_ReturnsEmptyCollection()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            GetApplianceCategoriesDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_GetApplianceCategoriesAsync()));
            Assert.NotNull(dto);
            Assert.Empty(dto.Categories);
        }

        [Fact]
        public async Task GetApplianceCategories_AssignedCategories_ReturnsCorrectCategories()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory1", -1));
                context.ApplianceCategories.Add(new ApplianceCategory("TestCategory2", -1));
                context.SaveChanges();
            }

            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            GetApplianceCategoriesDto dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = await _client.Appliance_GetApplianceCategoriesAsync()));
            Assert.NotNull(dto);
            Assert.Equal(2, dto.Categories.Count());
            Assert.Contains(dto.Categories, x => x.CategoryName == "TestCategory1");
            Assert.Contains(dto.Categories, x => x.CategoryName == "TestCategory2");
        }

        [Fact]
        public async Task GetApplianceCategories_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Appliance_GetApplianceCategoriesAsync());
        }

        [Fact]
        public async Task GetApplianceActivity_ValidRequest_Succeeds()
        {
            (await _client.AuthenticateUserAsync("mwilson", "password")).AddAuthorization(_httpClient);

            IList<ApplianceActivityDto> dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = (await _client.Appliance_GetApplianceActivityAsync()).ToList()));
            Assert.NotNull(dto);
            Assert.NotEmpty(dto);

            foreach (var lastActive in dto)
            {
                Assert.InRange(lastActive.LastActivity, lastActive.LastActivity, DateTime.Now);
            }
        }

        [Fact]
        public async Task GetApplianceActivity_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCustomerAuthorization(_httpClient, () =>
                _client.Appliance_GetApplianceActivityAsync());
        }

        [Fact]
        public async Task GetCustomerApplianceActivity_AssignedCustomer_Succeeds()
        {
            using (var context = _factory.GetKIOTContext())
            {
                context.AddCaretakerForCustomer("jcole", "mwilson");
            }

            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            IList<ApplianceActivityDto> dto = null;
            Assert.Null(await Record.ExceptionAsync(async () =>
                dto = (await _client.Appliance_GetCustomerApplianceActivityAsync(
                    Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2"))).ToList()));
            Assert.NotNull(dto);
            Assert.NotEmpty(dto);

            foreach (var lastActive in dto)
            {
                Assert.InRange(lastActive.LastActivity, lastActive.LastActivity, DateTime.Now);
            }
        }

        [Fact]
        public async Task GetCustomerApplianceActivity_InvalidGuid_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            IList<ApplianceActivityDto> dto = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                dto = (await _client.Appliance_GetCustomerApplianceActivityAsync(Guid.NewGuid())).ToList()));
            Assert.Null(dto);
        }
        
        [Fact]
        public async Task GetCustomerApplianceActivity_NonAssignedCustomer_Fails()
        {
            (await _client.AuthenticateUserAsync("jcole", "password")).AddAuthorization(_httpClient);

            IList<ApplianceActivityDto> dto = null;
            Assert.NotNull(await Record.ExceptionAsync(async () =>
                dto = (await _client.Appliance_GetCustomerApplianceActivityAsync(
                    Guid.Parse("d53a5412-efdd-4905-9f5d-5c2a624726a2"))).ToList()));
            Assert.Null(dto);
        }

        [Fact]
        public async Task GetCustomerApplianceActivity_NoAuthorization_Fails()
        {
            await _client.AssertRequiresCaretakerAuthorization(_httpClient, () =>
                _client.Appliance_GetCustomerApplianceActivityAsync(Guid.NewGuid()));
        }
    }
}
