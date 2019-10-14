using System;
using FluentValidation;

namespace KIOT.Server.Dto.Application.Customers.Data
{
    public enum TimeInterval
    {
        Day = 1,
        Week = Day * 7,
        Month = Day * 30,
        Year = Day * 365
    }

    //TODO: Hacky way to fix NSwag CodeGeneration issues. Fix later.
    public enum TimeInterval2
    {
        Day = 1,
        Week = Day * 7,
        Month = Day * 30,
        Year = Day * 365
    }
    
    public class GetCustomerDetailedPageDto
    {
        public TimeInterval TimeInterval { get; set; }
        public ushort? IntervalOffset { get; set; }
    }

    public class GetCustomerDetailedPageDtoValidator : AbstractValidator<GetCustomerDetailedPageDto>
    {
        public GetCustomerDetailedPageDtoValidator()
        {
            RuleFor(x => x.TimeInterval).Must(x => Enum.IsDefined(typeof(TimeInterval), x));
        }
    }
}
