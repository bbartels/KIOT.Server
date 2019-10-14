using Microsoft.AspNetCore.Mvc;

using KIOT.Server.Core.Presenter;

namespace KIOT.Server.Api.Presenter
{
    internal static class PresenterExtensions
    {
        private const string JsonMediaType = "application/json";

        public static IActionResult ToIActionResult(this IPresenter source)
        {
            return new ContentResult
            {
                ContentType = JsonMediaType,
                Content = source.ToJson(),
                StatusCode = (int)source.StatusCode
            };
        }
    }
}
