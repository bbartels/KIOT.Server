using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Response.Application.Caretakers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Customers;

namespace KIOT.Server.Business.Handler.Application.Caretakers.Customers
{
    public class GetAssignedCustomersRequestHandler : IRequestHandler<GetAssignedCustomersRequest, GetAssignedCustomersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<GetAssignedCustomersRequest> _validator;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _unitOfWork;

        public GetAssignedCustomersRequestHandler(IMapper mapper, IValidator<GetAssignedCustomersRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> unitOfWork)
        {
            _mapper = mapper;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetAssignedCustomersResponse> Handle(GetAssignedCustomersRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetAssignedCustomersResponse(new [] { new Error("InvalidRequest", "Could not process request.") });
            }

            var response = await _unitOfWork.Repository.GetAsync(x => x.Caretaker.Guid == request.CaretakerGuid,
                x => x.OrderBy(cfc => cfc.Customer.LastName), $"{nameof(Customer)}");

            var mapped = _mapper.Map<IEnumerable<CustomerInfoDto>>(response.Select(x => x.Customer));

            return new GetAssignedCustomersResponse(new AssignedCustomersForCaretakerDto { Customers = mapped});
        }
    }
}
