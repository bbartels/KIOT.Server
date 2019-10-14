using System;
using FluentValidation;

using KIOT.Server.Dto.Application.Customers.Data;

namespace KIOT.Server.Dto.Application.Caretakers.Customers
{
    public class GetApplianceHistoryDto
    {
        public Guid CustomerGuid { get; set; }
        public TimeInterval TimeInterval { get; set; }
        public ushort? IntervalOffset { get; set; }
    }

    public class GetApplianceHistoryRequestDtoValidator : AbstractValidator<GetApplianceHistoryDto>
    {
        public GetApplianceHistoryRequestDtoValidator()
        {
            RuleFor(x => x.TimeInterval).Must(x => Enum.IsDefined(typeof(TimeInterval), x));
        }
    }
}
