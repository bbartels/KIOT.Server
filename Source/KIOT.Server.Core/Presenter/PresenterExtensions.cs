using System.Linq;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto;

namespace KIOT.Server.Core.Presenter
{
    internal static class PresenterExtensions
    {
        public static BadRequestDto ToBadRequestDto(this CommandResponse source)
        {
            return new BadRequestDto(source.Errors.ToDictionary(x => x.Code, x => x.Description), source.Message);
        }

        public static SuccessfulRequestDto ToSuccessfulRequestDto(this CommandResponse source)
        {
            return new SuccessfulRequestDto(source.Message);
        }
    }
}
