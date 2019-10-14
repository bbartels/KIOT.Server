using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using KIOT.Server.Business.Request.Application.Customer.Caretakers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using MediatR;

namespace KIOT.Server.Business.Handler.Application.Customers.Caretakers
{
    internal class DeleteCaretakerForCustomerRequestHandler :
        IRequestHandler<DeleteCaretakerForCustomerRequest, CommandResponse>
    {
        private readonly IValidator<DeleteCaretakerForCustomerRequest> _validator;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _unitOfWork;

        public DeleteCaretakerForCustomerRequestHandler(IValidator<DeleteCaretakerForCustomerRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> unitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(DeleteCaretakerForCustomerRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", ""));
            }

            var cfc = await _unitOfWork.Repository.SingleOrDefaultAsync(
                x => x.Customer.Guid == request.CustomerGuid && x.Caretaker.Guid == request.CaretakerGuid);

            if (cfc == null)
            {
                return new CommandResponse(new Error("InvalidCaretaker", "Caretaker does not exist."));
            }

            try
            {
                _unitOfWork.Repository.Remove(cfc);
                await _unitOfWork.SaveChangesAsync();
                return new CommandResponse { Message = "Successfully deleted Caretaker!" };
            }

            catch (Exception) { return new CommandResponse(new Error("ServerError", "ServerError"));}
        }
    }
}
