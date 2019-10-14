using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class SetApplianceCategoryRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public int ApplianceId { get; }
        public string CategoryName { get; }

        public SetApplianceCategoryRequest(int applianceId, string categoryName, ClaimsIdentity identity) : base(identity)
        {
            ApplianceId = applianceId;
            CategoryName = categoryName;
        }
    }

    public class SetApplianceCategoryRequestValidator : AbstractValidator<SetApplianceCategoryRequest>
    {
        public SetApplianceCategoryRequestValidator() { }
    }
}
