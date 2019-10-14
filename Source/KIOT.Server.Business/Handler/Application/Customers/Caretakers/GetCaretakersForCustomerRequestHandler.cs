using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Customer.Caretakers;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Customers.Caretakers;

namespace KIOT.Server.Business.Handler.Application.Customers.Caretakers
{
    internal class GetCaretakersForCustomerRequestHandler : IRequestHandler<GetCaretakersForCustomerRequest, GetCaretakersForCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<GetCaretakersForCustomerRequest> _validator;
        private readonly IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> _unitOfWork;

        public GetCaretakersForCustomerRequestHandler(IMapper mapper,
            IValidator<GetCaretakersForCustomerRequest> validator,
            IUnitOfWork<ICaretakerForCustomerRepository, CaretakerForCustomer> unitOfWork)
        {
            _mapper = mapper;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCaretakersForCustomerResponse> Handle(GetCaretakersForCustomerRequest request,
            CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetCaretakersForCustomerResponse(new []{ new Error("AuthenticationFailed", "Could not authenticate User.") });
            }

            var caretakers = (await _unitOfWork.Repository.GetAsync(x => x.Customer.Guid == request.CustomerGuid,
                    x => x.OrderBy(y => y.Caretaker.LastName), $"{nameof(Caretaker)}")).Select(x => x.Caretaker);

            var response = _mapper.Map<IEnumerable<CaretakerInfoDto>>(caretakers);

            return new GetCaretakersForCustomerResponse(new AssignedCaretakersForCustomerDto{ Caretakers = response });
        }
    }
}
