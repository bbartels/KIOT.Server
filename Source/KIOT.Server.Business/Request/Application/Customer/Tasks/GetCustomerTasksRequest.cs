using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Tasks;

namespace KIOT.Server.Business.Request.Application.Customer.Tasks
{
    public class GetCustomerTasksRequest : AuthenticatedCustomerRequest<GetCustomerTasksResponse>
    {
        public GetCustomerTasksDto Dto { get; private set; }

        public GetCustomerTasksRequest(GetCustomerTasksDto dto, ClaimsIdentity identity) : base(identity)
        {
            Dto = dto;
        }
    }

    public class GetCustomerTasksRequestValidator : AbstractValidator<GetCustomerTasksRequest>
    {
        public GetCustomerTasksRequestValidator()
        {
            RuleFor(x => x.Dto.TaskOption).IsInEnum();
        }
    }
}
