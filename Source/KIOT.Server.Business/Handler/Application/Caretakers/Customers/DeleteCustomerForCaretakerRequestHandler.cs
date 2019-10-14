using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using MediatR;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    internal class DeleteCustomerForCaretakerRequestHandler : 
        IRequestHandler<DeleteCustomerForCaretakerRequest, CommandResponse>
    {
        private readonly IValidator<DeleteCustomerForCaretakerRequest> _validator;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _unitOfWork;

        public DeleteCustomerForCaretakerRequestHandler(IValidator<DeleteCustomerForCaretakerRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> unitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(DeleteCustomerForCaretakerRequest request,
            CancellationToken cancellationToken)
        {
            if(!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", ""));
            }

            var cfc = await _unitOfWork.Repository.SingleOrDefaultAsync(
                x => x.Customer.Guid == request.CustomerGuid && x.Caretaker.Guid == request.CaretakerGuid);

            if (cfc == null)
            {
                return new CommandResponse(new Error("InvalidCustomer", "Customer does not exist."));
            }

            try
            {
                _unitOfWork.Repository.Remove(cfc);
                await _unitOfWork.SaveChangesAsync();
                return new CommandResponse { Message = "Successfully deleted Customer!" };
            }

            catch (Exception) { return new CommandResponse(new Error("ServerError", "ServerError"));}
        }
    }
}
