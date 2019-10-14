using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Response.Application.Caretakers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using MediatR;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    internal class GetPendingCaretakerRequestsRequestHandler :
        IRequestHandler<GetPendingCaretakerRequestsRequest, GetPendingCaretakerRequestsResponse>

    {
        private readonly IMapper _mapper;
        private readonly IValidator<GetPendingCaretakerRequestsRequest> _validator;
        private readonly
            IUnitOfWork<ICaretakerForCustomerRequestRepository, CaretakerForCustomerRequest> _unitOfWork;

        public GetPendingCaretakerRequestsRequestHandler(IMapper mapper,
            IValidator<GetPendingCaretakerRequestsRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRequestRepository, CaretakerForCustomerRequest> unitOfWork)
        {
            _mapper = mapper;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetPendingCaretakerRequestsResponse> Handle(GetPendingCaretakerRequestsRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetPendingCaretakerRequestsResponse(new [] { new Error("InvalidRequest", "Could not process request.") });
            }

            var result = await _unitOfWork.Repository.GetRequests(request.Identity.Name);

            var mapped = _mapper.Map<IEnumerable<CaretakerForCustomerRequestDto>>(result.Entity);

            return new GetPendingCaretakerRequestsResponse(new PendingCaretakerRequestsDto { CaretakerRequests = mapped});
        }
    }
}
