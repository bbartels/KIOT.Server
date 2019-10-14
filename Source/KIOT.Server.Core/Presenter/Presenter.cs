using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Core.Presenter
{
    public class Presenter<TResponse> : IPresenter where TResponse : CommandResponse
    {
        public HttpStatusCode StatusCode { get; protected set; }
        public object Content { get; protected set; }

        public Presenter(TResponse response)
        {
            if (StatusCode == 0)
            {
                StatusCode = response.Succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            }

            if (Content == null && response.GetType() == typeof(CommandResponse))
            {
                Content = response.Succeeded
                    ? response.ToSuccessfulRequestDto()
                    : (object) response.ToBadRequestDto();
            }

            else if (Content == null && response.Succeeded) { Content = response.ObjectResult; }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(Content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
