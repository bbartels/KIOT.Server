using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    public class HandleCaretakerRequestRequestHandler : IRequestHandler<HandleCaretakerRequestRequest, CommandResponse>
    {
        private readonly IValidator<HandleCaretakerRequestRequest> _validator;

        private readonly IUnitOfWork<ICaretakerForCustomerRequestRepository, CaretakerForCustomerRequest> _cfcrUnitOfWork;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _cfcUnitOfWork;

        public HandleCaretakerRequestRequestHandler(IValidator<HandleCaretakerRequestRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRequestRepository, CaretakerForCustomerRequest> cfcrUnitOfWork,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> cfcUnitOfWork)
        {
            _validator = validator;
            _cfcrUnitOfWork = cfcrUnitOfWork;
            _cfcUnitOfWork = cfcUnitOfWork;
        }

        public async Task<CommandResponse> Handle(HandleCaretakerRequestRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "InvalidRequest"));
            }

            var response = await _cfcrUnitOfWork.Repository.CheckRequestExistsAndDelete(request.RequestId, request.Identity.Name);

            if (request.AcceptRequest)
            {
                if (!response.Succeeded) { return new CommandResponse(response.Errors); }

                var cfc = new CaretakerForCustomer(response.Entity.CaretakerId, response.Entity.CustomerId);

                try
                {
                    _cfcUnitOfWork.Repository.Add(cfc);
                    await _cfcUnitOfWork.SaveChangesAsync();
                    return new CommandResponse { Message = "Successfully accepted Request." };
                }

                catch (Exception)
                {
                    return new CommandResponse(new Error("CaretakerForCustomerExists",
                        "Could not accept request, since caretaker is already assigned to Customer"));
                }
            }

            return new CommandResponse { Message = "Successfully declined Request." };
        }
    }
}
