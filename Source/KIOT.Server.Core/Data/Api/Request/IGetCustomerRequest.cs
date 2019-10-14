namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IGetCustomerRequest : IApiRequest
    {
        ushort PageSize { get; }
        uint Offset { get; }
    }
}
