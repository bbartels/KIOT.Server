using System.Collections.Generic;

namespace KIOT.Server.Dto
{
    public class BadRequestDto
    {
        public IDictionary<string, string> Errors { get; }
        public string Message { get; }
        public int? ErrorCode { get; }

        public BadRequestDto(IDictionary<string, string> errors, string message, int? errorCode = null)
        {
            Errors = errors;
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
