namespace KIOT.Server.Dto.Application.Customers.Tasks
{
    public enum TaskOption : byte
    {
        Unfinished,
        Finished,
        Both
    }

    public class GetCustomerTasksDto
    {
        public TaskOption TaskOption { get; set; }
    }
}
