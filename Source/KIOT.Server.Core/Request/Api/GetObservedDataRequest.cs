using System;
using System.Collections.Generic;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Models.Data;

namespace KIOT.Server.Core.Request.Api
{
    public class GetObservedDataRequest : IGetObservedDataRequest
    {
        private const ApiRequestType RequestType = ApiRequestType.GetObservedData;
        private const string CustomerKeyName = "customer";
        private const string StartTimeKeyName = "sts";
        private const string EndTimeKeyName = "ets";
        private const string TimeUnitsKeyName = "time_units";
        private string DefaultEndTime => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        private string DefaultTimeUnit => new TimeUnitId(TimeUnit.Day).Id.ToString();

        public ApiRequestType Type => RequestType;

        public CustomerCode Customer { get; }
        public SensorTimestamp StartTimestamp { get; }
        public SensorTimestamp? EndTimestamp { get; }
        public TimeUnitId? TimeUnitId { get; }

        public GetObservedDataRequest(CustomerCode customer, SensorTimestamp start, SensorTimestamp? end = null, TimeUnitId? timeUnit = null)
        {
            Customer = customer;
            StartTimestamp = start;
            EndTimestamp = end;
            TimeUnitId = timeUnit;
        }

        public string BuildQueryString()
        {
            var dictionary = new Dictionary<string, string>
            {
                { CustomerKeyName, Customer.Code },
                { StartTimeKeyName, StartTimestamp.Ticks.ToString() },
                { EndTimeKeyName, EndTimestamp is SensorTimestamp ts ? ts.Ticks.ToString() : DefaultEndTime },
                { TimeUnitsKeyName, TimeUnitId is TimeUnitId tuId ? tuId.Id.ToString() : DefaultTimeUnit }
            };

            return RequestHelpers.BuildQueryString(dictionary);
        }
    }
}
