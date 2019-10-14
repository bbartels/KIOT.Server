using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request.Application.Customer.Appliances
{
    public class DeleteApplianceCategoryRequest : AuthenticatedCustomerRequest<CommandResponse>
    {
        public Guid CategoryGuid { get; }

        public DeleteApplianceCategoryRequest(Guid categoryGuid, ClaimsIdentity identity) : base(identity)
        {
            CategoryGuid = categoryGuid;
        }
    }

    public class DeleteApplianceCategoryRequestValidator : AbstractValidator<DeleteApplianceCategoryRequest>
    {

    }
}
