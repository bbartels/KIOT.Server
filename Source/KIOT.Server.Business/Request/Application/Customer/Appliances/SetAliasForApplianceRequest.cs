using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Appliances;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class SetAliasForApplianceRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public int ApplianceId { get; }
        public string Alias { get; }

        public SetAliasForApplianceRequest(ClaimsIdentity identity, SetAliasForApplianceDto dto) : base(identity)
        {
            ApplianceId = dto.ApplianceId;
            Alias = dto.Alias;
        }
    }

    public class SetAliasForApplianceRequestValidator : AbstractValidator<SetAliasForApplianceRequest>
    {
        public SetAliasForApplianceRequestValidator()
        {
            RuleFor(x => x.Alias).MinimumLength(3);
        }
    }
}
