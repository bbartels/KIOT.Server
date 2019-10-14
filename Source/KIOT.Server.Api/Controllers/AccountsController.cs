using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

using KIOT.Server.Business.Request.Application;
using KIOT.Server.Api.Presenter;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Attempts to register a new Caretaker based on passed details. 
        /// </summary>
        /// <param name="caretakerDto">The Caretaker details used to register the user.</param>
        /// <returns>Whether the registration was successful or not.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterCaretaker")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> RegisterCaretakerAsync(RegisterCaretakerDto caretakerDto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new RegisterCaretakerRequest(caretakerDto), token)).ToIActionResult();
        }

        /// <summary>
        /// Attempts to register a new Customer based on passed details. 
        /// </summary>
        /// <param name="customerDto">The Customer details used to register the user.</param>
        /// <returns>Whether the registration was successful or not.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterCustomer")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> RegisterCustomerAsync(RegisterCustomerDto customerDto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new RegisterCustomerRequest(customerDto), token)).ToIActionResult();
        }
    }
}
