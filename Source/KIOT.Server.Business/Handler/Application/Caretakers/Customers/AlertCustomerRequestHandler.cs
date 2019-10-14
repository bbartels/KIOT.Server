using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    public class AlertCustomerRequestHandler : IRequestHandler<AlertCustomerRequest, CommandResponse>
    {
        private readonly IValidator<AlertCustomerRequest> _validator;
        private readonly IPushNotificationService _notificationService;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;

        public AlertCustomerRequestHandler(IValidator<AlertCustomerRequest> validator,
            IPushNotificationService notificationService, IUnitOfWork<ICustomerRepository, Customer> unitOfWork)
        {
            _validator = validator;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(AlertCustomerRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new CommandResponse(validationResult);
            }

            var customer = await _unitOfWork.Repository.SingleOrDefaultAsync(c => c.Username == request.Dto.Username,
                $"{nameof(Customer.PushTokens)},{nameof(Customer.IsCaredForBy)}.{nameof(CaretakerForCustomerRequest.Caretaker)}");

            if (customer == null || !customer.IsCaredForByCaretaker(request.CaretakerGuid))
            {
                return new CommandResponse(new Error("InvalidRequest", "Invalid push notification request."));
            }

            var tokens = customer.GetValidPushTokens();
            await _notificationService.SendNotificationsAsync(tokens.Select(t => new PushNotification(t.Token, request.Dto.Message, "Alert")));

            return new CommandResponse();
        }
    }
}
