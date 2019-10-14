using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Customer.Caretakers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Handler.Application.Customers.Caretakers
{
    internal class AddCaretakerForCustomerRequestHandler : IRequestHandler<AddCaretakerForCustomerRequest, CommandResponse>
    {
        private readonly IValidator<AddCaretakerForCustomerRequest> _validator;
        private readonly IPushNotificationService _pushService;
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _cUnitOfWork;

        public AddCaretakerForCustomerRequestHandler(IValidator<AddCaretakerForCustomerRequest> validator, IPushNotificationService pushService,
            IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork, IUnitOfWork<ICustomerRepository, Customer> cUnitOfWork)
        {
            _validator = validator;
            _pushService = pushService;
            _unitOfWork = unitOfWork;
            _cUnitOfWork = cUnitOfWork;
        }

        public async Task<CommandResponse> Handle(AddCaretakerForCustomerRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("Invalid request", "Could not proceed with request. Invalid parameters"));
            }

            var caretakerTask = _unitOfWork.Repository.SingleOrDefaultAsync(x => x.Username == request.CaretakerUsername,
                $"{nameof(Caretaker.PushTokens)}," +
                $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}," +
                $"{nameof(Caretaker.TakingCareOfRequests)}.{nameof(CaretakerForCustomerRequest.Customer)}");
            var customerTask = _cUnitOfWork.Repository.GetIdAsync(x => x.Guid == request.CustomerGuid);

            await Task.WhenAll(customerTask, caretakerTask);
            var caretaker = caretakerTask.Result;

            if (caretaker == null)
            {
                return new CommandResponse(new[] { new Error("InvalidRequest", "Could not add Caretaker.") });
            }

            if (caretaker.IsTakingCareOf(request.CustomerGuid) || caretaker.HasPendingRequest(request.CustomerGuid))
            {
                return new CommandResponse(new Error("CaretakerForCustomerExists",
                    "Could not create request, caretaker is already registered with customer"));
            }

            if (!(customerTask.Result is int customerId)) { return new CommandResponse(new Error("ServerError", "Could not find Customer.")); }

            try
            {
                caretaker.AddCustomerRequest(customerId);
                await _unitOfWork.SaveChangesAsync();
                await _pushService.SendNotificationsAsync(caretaker, "New Customer Request",
                    "A customer just requested you to be their Caretaker");
                return new CommandResponse();
            }

            catch (Exception)
            {
                return new CommandResponse(new Error("Server Error", "Could not add Caretaker."));
            }
        }
    }
}
