using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Jobs;
using KIOT.Server.Business.Request.Application;
using KIOT.Server.Business.Response.Application;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Handler.Application
{
    internal class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly IUnitOfWork<IUserRepository, User> _userUnitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IBackgroundTaskScheduler _scheduler;
        private readonly IValidator<LoginUserRequest> _validator;

        public LoginUserRequestHandler(IValidator<LoginUserRequest> validator, IUnitOfWork<IUserRepository, User> userUnitOfWork,
            ITokenService tokenService, IBackgroundTaskScheduler scheduler)
        {
            _userUnitOfWork = userUnitOfWork;
            _tokenService = tokenService;
            _scheduler = scheduler;
            _validator = validator;
        }

        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var result = _validator.Validate(request);

            if (!result.IsValid) { return new LoginUserResponse(result.ToErrors(), "Invalid login credentials."); }

            var response = await _userUnitOfWork.Repository.GetUserByName(request.Username);
            if (!response.Succeeded) { return new LoginUserResponse(response.Errors, ""); }

            var user = response.Entity;
            var dataResponse = await _userUnitOfWork.Repository.CheckCredentials(user, request.Password);

            if (dataResponse.Succeeded)
            {
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.AddRefreshToken(refreshToken, user.IdentityId, IPAddress.Parse(request.RemoteAddress));
                _userUnitOfWork.Repository.Update(user);
                await _userUnitOfWork.SaveChangesAsync();

                var (token, expiresIn) = _tokenService.GenerateAccessToken(user, user.Claims);

                if (user is Caretaker) { _scheduler.ScheduleTask(new CacheCustomersForCaretakerRequest(user.Guid)); }
                if (user is Customer) { _scheduler.ScheduleTask(new CacheCustomerApplianceHistoryRequest(user.Guid)); }
                return new LoginUserResponse(token, expiresIn, refreshToken, "Successfully logged in.");
            }

            return new LoginUserResponse(dataResponse.Errors, "Could not authenticate user.");
        }
    }
}
