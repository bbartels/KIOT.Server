using System.Threading;
using System.Threading.Tasks;
using KIOT.Server.Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using KIOT.Server.Api.Presenter;
using KIOT.Server.Api.Presenter.Application;
using KIOT.Server.Business.Request.Application;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpRequestService _httpService;

        public AuthenticationController(IMediator mediator, IHttpRequestService httpService)
        {
            _mediator = mediator;
            _httpService = httpService;
        }

        /// <summary>
        /// Attempts to authenticate user based on passed credentials.
        /// </summary>
        /// <param name="loginDto">The user credentials to be validated.</param>
        /// <returns>Returns whether the authentication was successful.</returns>
        /// <response code="200">Returns <see cref="AccessTokenDto"/>.</response>
        /// <response code="400">Returns <see cref="BadRequestDto"/>.</response>
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(AccessTokenDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> AuthenticateAsync(LoginUserDto loginDto, CancellationToken token)
        {
            var remoteAddress = _httpService.GetRemoteIpAddress(Request.HttpContext.Connection.RemoteIpAddress);
            var response = await _mediator.Send(new LoginUserRequest(loginDto, remoteAddress), token);

            return new LoginUserPresenter(response).ToIActionResult();
        }

        /// <summary>
        /// Attempts to exchange RefreshToken for a new AccessToken in order to re-authenticate with Api.
        /// </summary>
        /// <param name="dto">The previously valid Access- and RefreshToken in order to exchange them for new valid ones.</param>
        /// <returns>A new Access- and RefreshToken on successfully validated previous tokens.</returns>
        [HttpPost("ExchangeToken")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(AccessTokenDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> ExchangeRefreshTokenAsync(ExchangeRefreshTokenDto dto, CancellationToken token)
        {
            var remoteAddress = _httpService.GetRemoteIpAddress(Request.HttpContext.Connection.RemoteIpAddress);
            var response = await _mediator.Send(new ExchangeRefreshTokenRequest(dto, remoteAddress), token);

            return new ExchangeRefreshTokenPresenter(response).ToIActionResult(); 
        }


        [HttpPost("RegisterPushToken")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> RegisterPushToken(RegisterPushTokenDto dto, CancellationToken token)
        {
            return new Presenter<CommandResponse>(await _mediator.Send(
                    new RegisterPushTokenRequest(dto, User.ToClaimsIdentity()), token))
                    .ToIActionResult();
        }
    }
}