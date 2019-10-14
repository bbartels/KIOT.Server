using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using ApplianceType = KIOT.Server.Core.Models.Application.ApplianceType;
using Customer = KIOT.Server.Core.Models.Application.Customer;

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    internal class SetAliasForApplianceRequestHandler : IRequestHandler<SetAliasForApplianceRequest, CommandResponse>
    {
        private readonly IValidator<SetAliasForApplianceRequest> _validator;
        private readonly IApplianceService _applianceService;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;
        private readonly IUnitOfWork<IApplianceTypeRepository, ApplianceType> _apUnitOfWork;

        public SetAliasForApplianceRequestHandler(IValidator<SetAliasForApplianceRequest> validator,
            IApplianceService applianceService,
            IUnitOfWork<ICustomerRepository, Customer> unitOfWork,
            IUnitOfWork<IApplianceTypeRepository, ApplianceType> apUnitOfWork)
        {
            _validator = validator;
            _applianceService = applianceService;
            _unitOfWork = unitOfWork;
            _apUnitOfWork = apUnitOfWork;
        }

        public async Task<CommandResponse> Handle(SetAliasForApplianceRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "Parameters are incorrect."));
            }

            var applianceRegistered =
                await _unitOfWork.Repository.IsApplianceRegisteredWithCustomer(request.CustomerGuid, request.ApplianceId);

            if (applianceRegistered.Entity)
            {
                return new CommandResponse(await _unitOfWork.Repository.SetAliasForCustomerAppliance(request.CustomerGuid,
                    request.ApplianceId, request.Alias));
            }

            var customerTask = _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid, $"{nameof(Customer.Appliances)}");
            var applianceTask = _applianceService.GetCustomerAppliance(request.CustomerGuid, request.ApplianceId);

            await Task.WhenAll(customerTask, applianceTask);

            var appliance = applianceTask.Result;
            var customer = customerTask.Result;

            if (appliance == null)
            {
                return new CommandResponse(new Error("InvalidRequest", "Could not find remote registered appliance with given id."));
            }

            var typeId = appliance.ApplianceTypeId;

            var applianceType = await _apUnitOfWork.Repository.SingleOrDefaultAsync(x => x.ApplianceTypeId == typeId);

            // TODO: Get actual name from API and store in ApplianceType
            applianceType ??= new ApplianceType(typeId, null);

            customer.Appliances.Add(new CustomerAppliance(request.ApplianceId, request.Alias, customer.Id, applianceType));

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new CommandResponse(new Error("ServerError", "Could not update Appliance Alias."));
            }

            return new CommandResponse();
        }
    }
}
