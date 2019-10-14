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

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    internal class SetApplianceCategoryRequestHandler : IRequestHandler<SetApplianceCategoryRequest, CommandResponse>
    {
        private readonly IValidator<SetApplianceCategoryRequest> _validator;
        private readonly IApplianceService _service;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;
        private readonly IUnitOfWork<IApplianceTypeRepository, ApplianceType> _atUnitOfWork;

        public SetApplianceCategoryRequestHandler(IValidator<SetApplianceCategoryRequest> validator, IApplianceService service,
            IUnitOfWork<ICustomerRepository, Customer> unitOfWork,
            IUnitOfWork<IApplianceTypeRepository, ApplianceType> atUnitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
            _service = service;
            _atUnitOfWork = atUnitOfWork;
        }

        public async Task<CommandResponse> Handle(SetApplianceCategoryRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "Unable to process request."));
            }

            var customerTask = _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid,
                $"{nameof(Customer.Appliances)},{nameof(Customer.ApplianceCategories)}");

            var applianceTask = _service.GetCustomerAppliance(request.CustomerGuid, request.ApplianceId);

            await Task.WhenAll(customerTask, applianceTask);

            var customer = customerTask.Result;
            var appliance = applianceTask.Result;

            if (appliance == null)
            {
                return new CommandResponse(new Error("NoSuchAppliance", "Could not find requested Appliance."));
            }

            if (request.CategoryName == null && customer.HasAppliance(request.ApplianceId))
            {
                if (!customer.ResetApplianceCategory(request.ApplianceId))
                {
                    return new CommandResponse(new Error("ServerError", "ServerError"));
                }
            }

            else
            {
                if (!customer.HasApplianceCategory(request.CategoryName))
                {
                    return new CommandResponse(new Error("NoSuchCategory",
                        $"No category: {request.CategoryName} is registered."));
                }

                if (customer.HasAppliance(request.ApplianceId))
                {
                    if (!customer.SetApplianceCategory(request.ApplianceId, request.CategoryName))
                    {
                        return new CommandResponse(new Error("ServerError", "Could not set category name."));
                    }
                }

                else
                {
                    var applianceType =
                        await _atUnitOfWork.Repository.SingleOrDefaultAsync(x =>
                            x.ApplianceTypeId == appliance.ApplianceTypeId);

                    var customerAppliance = new CustomerAppliance(request.ApplianceId, appliance.ApplianceName,
                        customer.Id,
                        applianceType ?? new ApplianceType(appliance.ApplianceTypeId, null));

                    customer.AddAppliance(customerAppliance);

                    if (!customer.SetApplianceCategory(request.ApplianceId, request.CategoryName))
                    {
                        return new CommandResponse(new Error("ServerError", "Could not set category name."));
                    }
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new CommandResponse();
            }

            catch (Exception)
            {
                return new CommandResponse(new Error("ServerError", "ServerError"));
            }
        }
    }
}
