using System;
using System.Security.Claims;
using FluentValidation;

using KIOT.Server.Business.Response.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Business.Request.Application.Caretaker.Customers
{
    public class GetApplianceHistoryRequest : AuthenticatedCaretakerRequest<GetApplianceHistoryResponse>
    {
        public Guid CustomerGuid { get; }
        public TimeInterval Interval { get; }
        public ushort IntervalOffset { get; }

        public GetApplianceHistoryRequest(ClaimsIdentity identity, GetApplianceHistoryDto dto) : base(identity)
        {
            CustomerGuid = dto.CustomerGuid;
            Interval = dto.TimeInterval;
            IntervalOffset = dto.IntervalOffset ?? 0;
        }
    }

    public class GetApplianceHistoryRequestValidator : AbstractValidator<GetApplianceHistoryRequest>
    {
        public GetApplianceHistoryRequestValidator()
        {
            RuleFor(x => x.Interval).Must(x => Enum.IsDefined(typeof(TimeInterval), x));
        }
    }
}
