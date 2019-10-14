using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Response.Application.Caretakers.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    internal class GetCustomerApplianceActivityRequestHandler :
        IRequestHandler<GetCustomerApplianceActivityRequest, GetCustomerApplianceActivityResponse>
    {
        private readonly IValidator<GetCustomerApplianceActivityRequest> _validator;
        private readonly IApplianceService _applianceService;
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;

        public GetCustomerApplianceActivityRequestHandler(IValidator<GetCustomerApplianceActivityRequest> validator,
            IApplianceService applianceService, IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork)
        {
            _validator = validator;
            _applianceService = applianceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCustomerApplianceActivityResponse> Handle(GetCustomerApplianceActivityRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetCustomerApplianceActivityResponse(new [] { new Error("InvalidRequest", "InvalidRequest")  });
            }

            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid,
                $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}");

            return !caretaker.IsTakingCareOf(request.CustomerGuid)
                ? new GetCustomerApplianceActivityResponse(new [] { new Error("NoSuchCustomer", "Customer is not registered with Caretaker.") })
                : new GetCustomerApplianceActivityResponse(await _applianceService.GetCustomerAppliancesActivity(request.CustomerGuid));
        }
    }
}
