using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker.Tasks;
using KIOT.Server.Business.Response.Application.Caretakers.Tasks;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Tasks;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Tasks;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Tasks
{
    internal class GetCustomerTasksRequestHandler : IRequestHandler<GetCustomerTasksRequest, GetCustomerTasksResponse>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<GetCustomerTasksRequest> _validator;
        private readonly IUnitOfWork<ICaretakerRepository, Caretaker> _unitOfWork;

        public GetCustomerTasksRequestHandler(IMapper mapper, IValidator<GetCustomerTasksRequest> validator,
            IUnitOfWork<ICaretakerRepository, Caretaker> unitOfWork)
        {
            _mapper = mapper;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCustomerTasksResponse> Handle(GetCustomerTasksRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new GetCustomerTasksResponse(validationResult.ToErrors());
            }

            var caretaker = await _unitOfWork.Repository.GetByGuidAsync(request.CaretakerGuid, $"{nameof(Caretaker.CustomersTasks)}.{nameof(CustomerTask.Customer)}," +
                                                                                         $"{nameof(Caretaker.TakingCareOf)}.{nameof(CaretakerForCustomer.Customer)}");

            if (!caretaker.IsTakingCareOf(request.CustomerGuid))
            {
                return new GetCustomerTasksResponse(new [] { new Error("InvalidRequest", $"Could not find Customer with Guid: {request.CustomerGuid}") });
            }

            var customerTasks = caretaker.GetCustomersTasks(request.CustomerGuid);

            return new GetCustomerTasksResponse(new CustomerTasksForCaretakerDto
            {
                Customer = _mapper.Map<CustomerInfoDto>(caretaker.GetCustomer(request.CustomerGuid)),
                Tasks = _mapper.Map<IEnumerable<CustomerTaskDto>>(customerTasks)
            });
        }
    }
}
