namespace KIOT.Server.Core.Data.Api.Request
{
    public interface IRequestBuilder
    {
        string BuildRequest(IApiRequest request);
    }
}
