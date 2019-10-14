using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using KIOT.Server.Business.Abstractions.Services;
using KIOT.Server.Business.Jobs;
using MediatR;

using KIOT.Server.Business.Request.Application;
using KIOT.Server.Business.Response.Application;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Core.Services;

namespace KIOT.Server.Business.Handler.Application
{
    internal class ExchangeRefreshTokenRequestHandler : IRequestHandler<ExchangeRefreshTokenRequest, ExchangeRefreshTokenResponse>
    {
        private readonly IUnitOfWork<IUserRepository, User> _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IBackgroundTaskScheduler _scheduler;
        private readonly IValidator<ExchangeRefreshTokenRequest> _validator;

        public ExchangeRefreshTokenRequestHandler(IValidator<ExchangeRefreshTokenRequest> validator, IUnitOfWork<IUserRepository, User> unitOfWork,
            ITokenService tokenService, IBackgroundTaskScheduler scheduler)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _tokenService = tokenService;
            _scheduler = scheduler;
        }

        public async Task<ExchangeRefreshTokenResponse> Handle(ExchangeRefreshTokenRequest request,
                                                                CancellationToken cancellationToken)
        {
            var result = _validator.Validate(request);
            if (!result.IsValid)
            {
                return new ExchangeRefreshTokenResponse(result.ToErrors(), "Could not verify token");
            }

            var claims = _tokenService.GetUserClaims(request.AccessToken);
            var identityClaim = claims.FirstOrDefault(x => x.Type == _tokenService.IdentityIdClaimType);

            if (identityClaim == null)
            {
                return new ExchangeRefreshTokenResponse(
                    new[] { new Error("InvalidToken", "Token not in correct format.") }, "Could not verify token.");
            }

            var user = (await _unitOfWork.Repository
                .GetAsync(u => u.IdentityId == identityClaim.Value, null, $"{nameof(User.RefreshTokens)}")).SingleOrDefault();

            if (user == null)
            {
                return new ExchangeRefreshTokenResponse(
                    new[] { new Error("InvalidUser", "Could not verify user.") }, "Could not verify user.");
            }

            var claimsResponse = await _unitOfWork.Repository.GetClaimsAsync(user);

            if (user.CheckRefreshToken(request.RefreshToken) && claimsResponse.Succeeded)
            {
                var (token, expiresIn) = _tokenService.GenerateAccessToken(user, claimsResponse.Entity);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                user.RemoveRefreshToken(request.RefreshToken);
                user.AddRefreshToken(newRefreshToken, user.IdentityId, IPAddress.Parse(request.RemoteAddress));
                _unitOfWork.Repository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                if (user is Caretaker) { _scheduler.ScheduleTask(new CacheCustomersForCaretakerRequest(user.Guid)); }
                if (user is Customer) { _scheduler.ScheduleTask(new CacheCustomerApplianceHistoryRequest(user.Guid)); }

                return new ExchangeRefreshTokenResponse(token, expiresIn, newRefreshToken, "Successfully exchanged RefreshToken.");
            }

            return new ExchangeRefreshTokenResponse(
                new [] { new Error("InvalidRefreshToken", "Could not verify supplied RefreshToken.") }, "Could not verify supplied RefreshToken.");
        }
    }
}
