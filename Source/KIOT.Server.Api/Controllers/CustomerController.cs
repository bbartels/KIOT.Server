using System;
using System.Threading;
using System.Threading.Tasks;
using KIOT.Server.Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using KIOT.Server.Api.Presenter;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Business.Request.Application.Customer.Caretakers;
using KIOT.Server.Business.Request.Application.Customer.Data;
using KIOT.Server.Business.Request.Application.Customer.Tasks;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Caretakers;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Application.Customers.Tasks;

namespace KIOT.Server.Api.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Attempts to create a request which adds a Caretaker to the list or Caretakers of a Customer.
        /// The Customer has to make this request while being Authenticated.
        /// </summary>
        /// <param name="dto">The parameters required to make this request.</param>
        /// <returns>Whether the action succeeded or not.</returns>
        [HttpPost]
        [Route("AddCustomerCaretaker")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> AddCaretakerForCustomerAsync(AddCaretakerForCustomerDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new AddCaretakerForCustomerRequest(dto, User.ToClaimsIdentity()), token)).ToIActionResult();
        }

        [HttpGet]
        [Route("GetCustomerHomepage")]
        [ProducesResponseType(200, Type = typeof(CustomerHomepageDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetHomepageAsync(CancellationToken token)
        {
            return new Presenter<CustomerHomepageResponse>(await _mediator.Send(
                new GetCustomerHomepageRequest(User.ToClaimsIdentity()), token)).ToIActionResult();
        }

        [HttpGet]
        [Route("GetAssignedCaretakers")]
        [ProducesResponseType(200, Type = typeof(AssignedCaretakersForCustomerDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetAssignedCaretakers(CancellationToken token)
        {
            return new Presenter<GetCaretakersForCustomerResponse>(await 
                _mediator.Send(new GetCaretakersForCustomerRequest(User.ToClaimsIdentity()), token)).ToIActionResult();
        }

        [HttpDelete]
        [Route("DeleteAssignedCaretaker/{caretakerGuid}")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> DeleteAssignedCaretaker(Guid caretakerGuid, CancellationToken token)
        {
            return new Presenter<CommandResponse>(await
                _mediator.Send(new DeleteCaretakerForCustomerRequest(User.ToClaimsIdentity(), caretakerGuid), token))
                .ToIActionResult();
        }

        [HttpGet]
        [Route("GetDetailedPage")]
        [ProducesResponseType(200, Type = typeof(CustomerDetailedPageDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetDetailedPage([FromQuery]GetCustomerDetailedPageDto dto, CancellationToken token)
        {
            return new Presenter<GetDetailedCustomerPageResponse>(
                await _mediator.Send(new GetDetailedCustomerPageRequest(User.ToClaimsIdentity(),
                    dto), token)).ToIActionResult();
        }

        [HttpPost]
        [Route("SetAliasForAppliance")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> SetAliasForAppliance(SetAliasForApplianceDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(await
                _mediator.Send(new SetAliasForApplianceRequest(User.ToClaimsIdentity(), dto), token))
                .ToIActionResult();
        }

        [HttpGet]
        [Route("GetAppliances")]
        [ProducesResponseType(200, Type = typeof(CustomerAppliancesDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetAppliances(CancellationToken token)
        {
            return new Presenter<GetCustomerAppliancesResponse>(await
                _mediator.Send(new GetCustomerAppliancesRequest(User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpGet("GetAssignedTasks")]
        [ProducesResponseType(200, Type = typeof(CustomerTasksDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetAssignedTasks([FromQuery]GetCustomerTasksDto dto, CancellationToken token)
        {
            return new Presenter<GetCustomerTasksResponse>(await
                _mediator.Send(new GetCustomerTasksRequest(dto, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }
    }
}
