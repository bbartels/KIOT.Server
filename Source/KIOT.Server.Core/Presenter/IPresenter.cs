using System.Net;

namespace KIOT.Server.Core.Presenter
{
    public interface IPresenter
    {
        HttpStatusCode StatusCode { get; }
        string ToJson();
    }
}
