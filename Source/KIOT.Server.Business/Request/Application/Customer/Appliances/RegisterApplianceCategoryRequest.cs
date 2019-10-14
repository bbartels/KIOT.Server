using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class RegisterApplianceCategoryRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public string CategoryName { get; private set; }

        public RegisterApplianceCategoryRequest(string categoryName, ClaimsIdentity identity) : base(identity)
        {
            CategoryName = categoryName;
        }
    }

    public class RegisterApplianceCategoryRequestValidator : AbstractValidator<RegisterApplianceCategoryRequest>
    {
        public RegisterApplianceCategoryRequestValidator()
        {
            RuleFor(x => x.CategoryName).NotNull().NotEmpty();
        }
    }
}
