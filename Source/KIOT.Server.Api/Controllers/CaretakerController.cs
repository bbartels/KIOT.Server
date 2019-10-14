using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using KIOT.Server.Api.Helpers;
using KIOT.Server.Api.Presenter;
using KIOT.Server.Business.Response.Application.Caretakers;
using KIOT.Server.Business.Response.Application.Caretakers.Tasks;
using KIOT.Server.Business.Response.Application.Caretakers.Customers;
using KIOT.Server.Business.Request.Application.Caretaker;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Request.Application.Caretaker.Data;
using KIOT.Server.Business.Request.Application.Caretaker.Tasks;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Caretakers.Data;
using KIOT.Server.Dto.Application.Caretakers.Tasks;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Api.Controllers
{
    [Authorize(Roles = "Caretaker")]
    [Route("api/[controller]")]
    [ApiController]
    public class CaretakerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaretakerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all pending requests from Customers to register the User making
        /// the call, as their Caretaker. Requires the caller to have a Caretaker Role.
        /// </summary>
        /// <returns>A List of pending requests as <see cref="PendingCaretakerRequestsDto"/>.</returns>
        [HttpGet]
        [Route("GetPendingCaretakerRequests")]
        [ProducesResponseType(200, Type = typeof(PendingCaretakerRequestsDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetPendingCaretakerRequestsAsync()
        {
            return new Presenter<GetPendingCaretakerRequestsResponse>(
                await _mediator.Send(new GetPendingCaretakerRequestsRequest
                    (HttpContext.User.ToClaimsIdentity()))).ToIActionResult();
        }

        /// <summary>
        /// Endpoint to accept or decline a CaretakerForCustomerRequest.
        /// Will delete the CFCRequest after successfully handling the request.
        /// </summary>
        /// <param name="requestId">The request to be handled.</param>
        /// <param name="dto">Dto whether to accept or decline request.</param>
        /// <returns>Whether the operation was successful.</returns>
        [HttpPost]
        [Route("HandleCaretakerRequest/{requestId}")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> HandleCaretakerRequestAsync(Guid requestId, HandleCaretakerForCustomerRequestDto dto)
        {
            return new Presenter<CommandResponse>(await _mediator.Send(
                    new HandleCaretakerRequestRequest(requestId, dto.AcceptRequest,
                        HttpContext.User.ToClaimsIdentity()))).ToIActionResult();
        }

        [HttpGet]
        [Route("GetAssignedCustomers")]
        [ProducesResponseType(200, Type = typeof(AssignedCustomersForCaretakerDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetAssignedCustomersAsync()
        {
            return new Presenter<GetAssignedCustomersResponse>(await _mediator.Send(
                new GetAssignedCustomersRequest(HttpContext.User.ToClaimsIdentity()))).ToIActionResult();
        }

        [HttpGet]
        [Route("GetCustomerHomepage/{customerGuid}")]
        [ProducesResponseType(200, Type = typeof(CustomerHomepageForCaretakerDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetCustomerHomepage(Guid customerGuid)
        {
            return new Presenter<CustomerHomepageForCaretakerResponse>(await _mediator.Send(
                new GetCustomerHomepageForCaretakerRequest(HttpContext.User.ToClaimsIdentity(),
                    customerGuid))).ToIActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerGuid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAssignedCustomer/{customerGuid}")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> DeleteAssignedCustomer(Guid customerGuid, CancellationToken token)
        {
            return new Presenter<CommandResponse>(await
                _mediator.Send(new DeleteCustomerForCaretakerRequest(User.ToClaimsIdentity(), customerGuid), token))
                .ToIActionResult();
        }

        [HttpPost("AlertCustomer")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> AlertCustomer(AlertCustomerDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(await
                _mediator.Send(new AlertCustomerRequest(dto, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpPost("RegisterCustomerTask")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> RegisterCustomerTask(RegisterCustomerTaskDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                    await _mediator.Send(new RegisterCustomerTaskRequest(dto, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpPost("SetCustomerTaskState")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> SetCustomerTaskState(SetCustomerTaskStateDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                    await _mediator.Send(new SetCustomerTaskStateRequest(dto, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpPost("GetCustomerTasks")]
        [ProducesResponseType(200, Type = typeof(CustomerTasksForCaretakerDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetCustomerTasks(Guid userGuid, CancellationToken token)
        {
            return new Presenter<GetCustomerTasksResponse>(
                    await _mediator.Send(new GetCustomerTasksRequest(userGuid, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpGet("GetCustomerApplianceHistory")]
        [ProducesResponseType(200, Type = typeof(CustomerDetailedPageDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetCustomerApplianceHistory([FromQuery]GetApplianceHistoryDto dto, CancellationToken token)
        {
            return new Presenter<GetApplianceHistoryResponse>(await _mediator.Send(new GetApplianceHistoryRequest(User.ToClaimsIdentity(), dto), token))
                .ToIActionResult();
        }
    }
}