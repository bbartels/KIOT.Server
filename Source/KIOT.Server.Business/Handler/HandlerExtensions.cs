using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler
{
    internal static class HandlerExtensions
    {
        public static IEnumerable<Error> ToErrors(this ValidationResult result)
        {
            return result.Errors.Select(e => new Error(e.ErrorCode, e.ErrorMessage));
        }
    }
}
