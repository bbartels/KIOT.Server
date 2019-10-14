using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Customers;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Request.Application.Customer.Data
{
    public class GetDetailedCustomerPageRequest : AuthenticatedCustomerRequest<GetDetailedCustomerPageResponse>
    {
        public TimeInterval Interval { get; }
        public ushort IntervalOffset { get; }

        public GetDetailedCustomerPageRequest(ClaimsIdentity identity, GetCustomerDetailedPageDto dto) : base(identity)
        {
            Interval = dto.TimeInterval;
            IntervalOffset = dto.IntervalOffset ?? 0;
        }
    }

    public class GetDetailedCustomerPageRequestValidator :
        AuthenticatedCustomerRequestValidator<GetDetailedCustomerPageRequest>
    {
        public GetDetailedCustomerPageRequestValidator()
        {
            RuleFor(x => x.Interval).Must(x => Enum.IsDefined(typeof(TimeInterval), x));
        }
    }
}
