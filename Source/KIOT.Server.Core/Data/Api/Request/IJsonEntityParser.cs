using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IJsonEntityParser
    {
        CustomerDataResponse ConvertCustomer(string json);
        ObservedDataResponse ConvertObservedData(string json);
    }
}
