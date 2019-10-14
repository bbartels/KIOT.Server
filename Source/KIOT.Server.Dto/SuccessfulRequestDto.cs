namespace KIOT.Server.Dto
{
    public class SuccessfulRequestDto
    {
        public string Message { get; }

        public SuccessfulRequestDto(string message) { Message = message; }
    }
}
