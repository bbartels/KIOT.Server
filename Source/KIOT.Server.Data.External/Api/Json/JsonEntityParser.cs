using Newtonsoft.Json;

using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Data.External.Api.Json
{
    internal class JsonEntityParser : IJsonEntityParser
    {
        public CustomerDataResponse ConvertCustomer(string json)
        {
            return JsonConvert.DeserializeObject<JsonCustomerResponse>(json)
                .ToCustomerDataResponse();
        }

        public ObservedDataResponse ConvertObservedData(string json)
        {
            return JsonConvert.DeserializeObject<JsonObservedDataResponse>(json)
                .ToObservedDataResponse();
        }
    }
}
