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
    internal class DeleteApplianceCategoryRequestHandler : IRequestHandler<DeleteApplianceCategoryRequest, CommandResponse>
    {
        private readonly IValidator<DeleteApplianceCategoryRequest> _validator;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;

        public DeleteApplianceCategoryRequestHandler(IValidator<DeleteApplianceCategoryRequest> validator, IUnitOfWork<ICustomerRepository, Customer> unitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(DeleteApplianceCategoryRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "InvalidRequest"));
            }

            var customer = await _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid,
                $"{nameof(Customer.ApplianceCategories)},{nameof(ApplianceCategory.Appliances)}");

            var category = customer.GetCategory(request.CategoryGuid);

            if (category == null)
            {
                return new CommandResponse(new Error("NoSuchCategory", "No such category is registered with User."));
            }

            if (category.HasAppliances())
            {
                return new CommandResponse(new Error("CategoryHasAppliances", "Cannot delete Category as it has assigned Appliances."));
            }

            try
            {
                customer.RemoveApplianceCategory(category);
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
