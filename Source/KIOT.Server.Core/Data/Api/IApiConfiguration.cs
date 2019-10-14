namespace KIOT.Server.Core.Data.Api
{
    public interface IApiConfiguration
    {
        string Protocol { get; }
        string BaseUrl { get; }
        string Version { get; }
        string BasicAuthenticationHeader { get; }
        string GetRequestPath(ApiRequestType request);
    }
}
