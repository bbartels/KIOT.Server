using System;
using System.Security.Claims;
using FluentValidation;
using MediatR;

using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Request
{
    public interface IAuthenticatedCustomerRequest : IAuthenticatedRequest
    {
        Guid CustomerGuid { get; }
    }

    public abstract class AuthenticatedCustomerRequest<TResponse> : AuthenticatedRequest<TResponse>, IAuthenticatedCustomerRequest
        where TResponse : CommandResponse
    {
        public Guid CustomerGuid => Identity.GetGuid();

        protected AuthenticatedCustomerRequest(ClaimsIdentity identity) : base(identity) { }
    }

    public abstract class AuthenticatedCustomerRequestValidator<TRequest> : 
        AbstractValidator<TRequest> where TRequest : IAuthenticatedCustomerRequest
    {
        protected AuthenticatedCustomerRequestValidator()
        {
            RuleFor(x => x.Identity).MustBeCustomer();
        }
    }
}
