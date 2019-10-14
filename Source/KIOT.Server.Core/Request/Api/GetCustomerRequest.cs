using System;
using System.Collections.Generic;

using KIOT.Server.Core.Data.Api;
using KIOT.Server.Core.Data.Api.Request;

namespace KIOT.Server.Core.Request.Api
{
    public readonly struct GetCustomerRequest : IGetCustomerRequest
    {
        private const ApiRequestType RequestType = ApiRequestType.GetCustomers;

        private const string OffsetKeyName = "offset";
        private const string PageSizeKeyName = "limit";
        private const uint DefaultOffset = 0;
        private const ushort DefaultPageSize = 0;
        private const ushort MaxPageSize = 10000;

        public ApiRequestType Type => RequestType;

        public ushort PageSize { get; }
        public uint Offset { get; }

        public GetCustomerRequest(ushort pageSize = DefaultPageSize, uint offset = DefaultOffset)
        {
            PageSize = Math.Min(MaxPageSize, pageSize);
            Offset = offset;
        }

        public string BuildQueryString()
        {
            var dictionary = new Dictionary<string, string>();
            if (PageSize != DefaultPageSize) { dictionary.Add(PageSizeKeyName, PageSize.ToString()); }
            if (Offset != DefaultOffset) { dictionary.Add(OffsetKeyName, Offset.ToString()); }

            return dictionary.Count == 0 ? string.Empty : RequestHelpers.BuildQueryString(dictionary);
        }
    }
}
