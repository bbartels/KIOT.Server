using System.Security.Claims;
using FluentValidation;
using MediatR;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request
{
    public interface IAuthenticatedRequest
    {
        ClaimsIdentity Identity { get; }
    }

    public abstract class AuthenticatedRequest<TResponse> : IRequest<TResponse> where TResponse : CommandResponse
    {
        public ClaimsIdentity Identity { get; }

        protected AuthenticatedRequest(ClaimsIdentity identity)
        {
            Identity = identity;
        }
    }

    public abstract class AuthenticatedRequestValidator<TRequest> : AbstractValidator<TRequest>
        where TRequest : IAuthenticatedRequest
    {
        protected AuthenticatedRequestValidator()
        {
            RuleFor(x => x.Identity).MustBeAuthenticated();
        }
    }
}
