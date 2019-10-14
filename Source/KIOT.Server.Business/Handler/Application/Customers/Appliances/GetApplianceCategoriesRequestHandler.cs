using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Business.Response.Application.Customers.Appliances;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Business.Handler.Application.Customers.Appliances
{
    public class GetApplianceCategoriesRequestHandler :
        IRequestHandler<GetApplianceCategoriesRequest, GetApplianceCategoriesResponse>

    {
        private readonly IValidator<GetApplianceCategoriesRequest> _validator;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;
        private readonly IMapper _mapper;

        public GetApplianceCategoriesRequestHandler(IMapper mapper, IUnitOfWork<ICustomerRepository, Customer> unitOfWork, IValidator<GetApplianceCategoriesRequest> validator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<GetApplianceCategoriesResponse> Handle(GetApplianceCategoriesRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.Validate(request).IsValid)
            {
                return new GetApplianceCategoriesResponse(new [] { new Error("", "") });
            }

            var customer = await _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid,
                $"{nameof(Customer.ApplianceCategories)}");

            var categories = _mapper.Map<IEnumerable<ApplianceCategoryDto>>(customer.ApplianceCategories);

            return new GetApplianceCategoriesResponse(new GetApplianceCategoriesDto { Categories = categories });
        }
    }
}
