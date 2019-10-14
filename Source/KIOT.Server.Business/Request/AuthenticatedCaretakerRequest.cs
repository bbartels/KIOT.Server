using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request
{
    public interface IAuthenticatedCaretakerRequest : IAuthenticatedRequest
    {
        Guid CaretakerGuid { get; }
    }

    public abstract class AuthenticatedCaretakerRequest<TResponse> : AuthenticatedRequest<TResponse>, IAuthenticatedCaretakerRequest
        where TResponse : CommandResponse
    {
        public Guid CaretakerGuid => Identity.GetGuid();

        protected AuthenticatedCaretakerRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public abstract class AuthenticatedCaretakerRequestValidator<TRequest> : 
        AbstractValidator<TRequest> where TRequest : IAuthenticatedCaretakerRequest
    {
        protected AuthenticatedCaretakerRequestValidator()
        {
            RuleFor(x => x.Identity).MustBeCaretaker();
        }
    }
}
