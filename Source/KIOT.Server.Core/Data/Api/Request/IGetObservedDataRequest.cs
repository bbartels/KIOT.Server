using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IGetObservedDataRequest : IApiRequest
    {
        CustomerCode Customer { get; }
        SensorTimestamp StartTimestamp { get; }
        SensorTimestamp? EndTimestamp { get; }
        TimeUnitId? TimeUnitId { get; }
    }
}
