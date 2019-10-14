using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application.Customer.Tasks;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Dto.Application.Customers.Tasks;
using KIOT.Server.Dto.Application.Tasks;

namespace KIOT.Server.Business.Handler.Application.Customers.Tasks
{
    internal class GetCustomerTasksRequestHandler : IRequestHandler<GetCustomerTasksRequest, GetCustomerTasksResponse>
    {
        private readonly IMapper _mapper;
        private readonly IValidator<GetCustomerTasksRequest> _validator;
        private readonly IUnitOfWork<ICustomerRepository, Customer> _unitOfWork;

        public GetCustomerTasksRequestHandler(IValidator<GetCustomerTasksRequest> validator, IMapper mapper,
            IUnitOfWork<ICustomerRepository, Customer> unitOfWork)
        {
            _mapper = mapper;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCustomerTasksResponse> Handle(GetCustomerTasksRequest request,
            CancellationToken cancellationToken)
        {
            var result = _validator.Validate(request);
            if (!result.IsValid)
            {
                return new GetCustomerTasksResponse(result.ToErrors());
            }

            var customer = await _unitOfWork.Repository.GetByGuidAsync(request.CustomerGuid,
                $"{nameof(Customer.Tasks)}.{nameof(CustomerTask.Caretaker)}");

            var tasks = new List<CustomerTask>();

            switch (request.Dto.TaskOption)
            {
                case TaskOption.Both:
                {
                    tasks.AddRange(customer.GetTasks());
                } break;
                case TaskOption.Finished:
                {
                    tasks.AddRange(customer.GetFinishedTasks());
                } break;
                case TaskOption.Unfinished:
                {
                    tasks.AddRange(customer.GetUnfinishedTasks());
                } break;

                default: { throw new ArgumentOutOfRangeException($"Unknown enum state {request.Dto.TaskOption}"); }
            }

            return new GetCustomerTasksResponse(new CustomerTasksDto { Tasks = tasks.Select(x => _mapper.Map<CustomerTaskDto>(x)) });
        }
    }
}
