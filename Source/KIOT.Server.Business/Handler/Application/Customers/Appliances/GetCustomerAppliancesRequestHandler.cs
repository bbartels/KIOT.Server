using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Core.Data.Api.Request;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Customers.Appliances;

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    internal class GetCustomerAppliancesRequestHandler : IRequestHandler<GetCustomerAppliancesRequest, GetCustomerAppliancesResponse>
    {
        private readonly IApiClient _client;
        private readonly IValidator<GetCustomerAppliancesRequest> _validator;
        private readonly IApplianceService _applianceService;

        public GetCustomerAppliancesRequestHandler(IApiClient client,
            IValidator<GetCustomerAppliancesRequest> validator, IApplianceService applianceService)
        {
            _client = client;
            _validator = validator;
            _applianceService = applianceService;
        }

        public async Task<GetCustomerAppliancesResponse> Handle(GetCustomerAppliancesRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetCustomerAppliancesResponse(new []{ new Error("InvalidRequest", "Parameters are incorrect.") });
            }

            var applianceInfoTask = _applianceService.GetApplianceInfo(request.CustomerGuid);
            var applianceTask = _applianceService.GetCustomerAppliances(request.CustomerGuid);
            await Task.WhenAll(applianceInfoTask, applianceTask);

            var applianceInfo = applianceInfoTask.Result;
            var appliances = applianceTask.Result.ToList();

            _applianceService.AddApplianceInfo(appliances, applianceInfo);

            return new GetCustomerAppliancesResponse(new CustomerAppliancesDto{ Appliances = appliances });
        }
    }
}
