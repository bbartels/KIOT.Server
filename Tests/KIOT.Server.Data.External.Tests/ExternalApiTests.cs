using System.Threading.Tasks;
using Xunit;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Models.Data;
using KIOT.Server.Core.Request.Api;
using KIOT.Server.Core.Services;
using KIOT.Server.Data.External.Api;
using KIOT.Server.Data.External.Api.Json;

using KIOT.Server.Data.External.Api.Request;

namespace KIOT.Server.Data.External.Tests
{
    public class ExternalApiTests
    {
        private static readonly IApiConfiguration Configuration = new TestApiConfiguration();
        private static readonly IApiHttpClientAccessor Accessor = new ExternalHttpClientAccessor(Configuration);
        private static readonly IRequestBuilder RequestBuilder = new ExternalRequestBuilder(Configuration);
        private static readonly IJsonEntityParser JsonParser = new JsonEntityParser();

        private readonly IApiClient _client;

        public ExternalApiTests()
        {
            _client = new ExternalApiClient(Accessor, RequestBuilder, JsonParser);
        }

        public async Task GetApiData_ConsecutiveRequests_AreEqual()
        {
            var customerRequest = new GetCustomerRequest(pageSize: 1, offset: 0);
            var observedRequest = new GetObservedDataRequest(new CustomerCode("-removed-"),
                new SensorTimestamp(1508976100L), new SensorTimestamp(1508976180L), new TimeUnitId(30));
            var task1 = _client.GetResponseAsync(customerRequest);
            var task2 = _client.GetResponseAsync(customerRequest);
            var task3 = _client.GetResponseAsync(observedRequest);
            var task4 = _client.GetResponseAsync(observedRequest);

            await Task.WhenAll(task1, task2, task3, task4);

            Assert.NotNull(task1.Result);
            Assert.NotNull(task2.Result);
            Assert.NotNull(task3.Result);
            Assert.NotNull(task4.Result);
        }

        public async Task GetApiData_GetDataAndParseResponse_Succeeds()
        {
            var apiRequest = new GetCustomerRequest(pageSize: 1, offset: 0);
            var request = await _client.GetResponseAsync(apiRequest);
            Assert.NotNull(request);
        }
    }
}
