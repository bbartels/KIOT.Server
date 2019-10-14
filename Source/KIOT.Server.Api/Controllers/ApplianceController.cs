using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

using KIOT.Server.Api.Helpers;
using KIOT.Server.Api.Presenter;
using KIOT.Server.Business.Response.Application.Caretakers.Customers;
using KIOT.Server.Business.Response.Application.Customers.Appliances;
using KIOT.Server.Business.Request.Application.Caretaker.Customers;
using KIOT.Server.Business.Request.Application.Customer.Appliances;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Core.Response;
using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Appliances;

namespace KIOT.Server.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplianceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplianceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [Route("RegisterApplianceCategory")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> RegisterApplianceCategory(string categoryName, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new RegisterApplianceCategoryRequest(categoryName, User.ToClaimsIdentity()), token)).ToIActionResult();
        }

        [HttpDelete]
        [Authorize(Roles = "Customer")]
        [Route("DeleteApplianceCategory")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> DeleteApplianceCategory(Guid categoryGuid, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new DeleteApplianceCategoryRequest(categoryGuid, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [Route("SetApplianceCategory/{applianceId}")]
        [ProducesResponseType(200, Type = typeof(SuccessfulRequestDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> SetApplianceCategory(int applianceId, string categoryName, CancellationToken token)
        {
            return new Presenter<CommandResponse>(
                await _mediator.Send(new SetApplianceCategoryRequest(applianceId, categoryName, User.ToClaimsIdentity()), token)).ToIActionResult();
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("GetApplianceCategories")]
        [ProducesResponseType(200, Type = typeof(GetApplianceCategoriesDto))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetApplianceCategories(CancellationToken token)
        {
            return new Presenter<GetApplianceCategoriesResponse>(
                    await _mediator.Send(new GetApplianceCategoriesRequest(User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("GetApplianceActivity")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ApplianceActivityDto>))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetApplianceActivity(CancellationToken token)
        {
            return new Presenter<GetApplianceActivityResponse>(
                    await _mediator.Send(new GetApplianceActivityRequest(User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }

        [HttpGet]
        [Authorize(Roles = "Caretaker")]
        [Route("GetCustomerApplianceActivity/{customerGuid}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ApplianceActivityDto>))]
        [ProducesResponseType(400, Type = typeof(BadRequestDto))]
        public async Task<IActionResult> GetCustomerApplianceActivity(Guid customerGuid, CancellationToken token)
        {
            return new Presenter<GetCustomerApplianceActivityResponse>(
                    await _mediator.Send(new GetCustomerApplianceActivityRequest(customerGuid, User.ToClaimsIdentity()), token))
                .ToIActionResult();
        }
    }
}
