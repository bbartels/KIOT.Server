using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Business.Response.Application.Customers.Appliances;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    internal class GetApplianceActivityRequestHandler : IRequestHandler<GetApplianceActivityRequest, GetApplianceActivityResponse>
    {
        private readonly IValidator<GetApplianceActivityRequest> _validator;
        private readonly IApplianceService _applianceService;

        public GetApplianceActivityRequestHandler(IValidator<GetApplianceActivityRequest> validator, IApplianceService applianceService)
        {
            _validator = validator;
            _applianceService = applianceService;
        }

        public async Task<GetApplianceActivityResponse> Handle(GetApplianceActivityRequest request,
            CancellationToken cancellationToken)
        {
            return !_validator.Validate(request).IsValid
                ? new GetApplianceActivityResponse(new []{ new Error("InvalidRequest", "InvalidRequest") })
                : new GetApplianceActivityResponse(await _applianceService.GetCustomerAppliancesActivity(request.CustomerGuid));
        }
    }
}
