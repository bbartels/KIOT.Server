using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    internal class RegisterApplianceCategoryRequestHandler : IRequestHandler<RegisterApplianceCategoryRequest, CommandResponse>
    {
        private readonly IValidator<RegisterApplianceCategoryRequest> _validator;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _cUnitOfWork;

        public RegisterApplianceCategoryRequestHandler(IValidator<RegisterApplianceCategoryRequest> validator,
            IUnitOfWork<ICustomerRepository, Customer> cUnitOfWork)
        {
            _validator = validator;
            _cUnitOfWork = cUnitOfWork;
        }

        public async Task<CommandResponse> Handle(RegisterApplianceCategoryRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "Could not register ApplianceCategory."));
            }

            var customer = (await _cUnitOfWork.Repository.GetByGuidAsync(request.CustomerGuid, $"{nameof(Customer.ApplianceCategories)}"));

            if (customer == null || !customer.AddApplianceCategory(request.CategoryName))
            {
                return new CommandResponse(new Error("ServerError", "Could not register ApplianceCategory."));
            }

            try
            {
                await _cUnitOfWork.SaveChangesAsync();
                return new CommandResponse();
            }
            catch (Exception)
            {
                return new CommandResponse(new Error("ServerError", "Could not register ApplianceCategory."));
            }
        }
    }
}
