namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IApiRequest
    {
        ApiRequestType Type { get; }
        string BuildQueryString();
    }
}
