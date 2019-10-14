using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker.Tasks;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;
using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Tasks
{
    public class SetCustomerTaskStateRequestHandler : IRequestHandler<SetCustomerTaskStateRequest, CommandResponse>
    {
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;
        private readonly IValidator<SetCustomerTaskStateRequest> _validator;
        private readonly IPushNotificationService _pushService;

        public SetCustomerTaskStateRequestHandler(IValidator<SetCustomerTaskStateRequest> validator, IPushNotificationService pushService,
            IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork)
        {
            _validator = validator;
            _pushService = pushService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResponse> Handle(SetCustomerTaskStateRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new CommandResponse(new Error("InvalidRequest", "InvalidRequest"));
            }

            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid,
                $"{nameof(Caretaker.CustomersTasks)}.{nameof(CustomerTask.Customer)}.{nameof(Customer.PushTokens)}");

            var customer = caretaker.CustomersTasks.SingleOrDefault(x => x.Guid == request.Dto.CustomerTaskGuid)?.Customer;

            if (customer == null)
            {
                return new CommandResponse(new Error("ServerError", "Could not set Task State."));
            }

            if (caretaker.GetTask(request.Dto.CustomerTaskGuid).TaskFinished)
            {
                return new CommandResponse(new Error("TaskAlreadyFinished", "Cannot cancel a finished task."));
            }

            caretaker.SetTaskState(request.Dto.CustomerTaskGuid, request.Dto.State);

            try
            {
                await _unitOfWork.SaveChangesAsync();

                string title;
                string message;

                switch (request.Dto.State)
                {
                    case CustomerTaskState.Complete:
                    {
                        title = "Task Completed";
                        message = "One of your tasks has been completed!";
                    } break;

                    case CustomerTaskState.Cancel:
                    {
                        title = "Task Cancelled";
                        message = $"{caretaker.Name} just cancelled one of your tasks!";
                    } break;

                    default: { throw new ArgumentOutOfRangeException(); }
                }


                await _pushService.SendNotificationsAsync(customer, title, message);
                return new CommandResponse("Successfully updated CustomerTask.");
            }
            catch (Exception)
            {
                return new CommandResponse(new Error("ServerError", "ServerError"));
            }
        }
    }
}
