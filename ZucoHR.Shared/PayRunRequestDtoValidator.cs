using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Shared
{    
    public class PayRunRequestDtoValidator : AbstractValidator<PayRunRequestDto>
    {
        public PayRunRequestDtoValidator()
        {
            RuleFor(x => x.PeriodStart)
                .LessThan(x => x.PeriodEnd)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.PeriodEnd)
                .GreaterThan(x => x.PeriodStart)
                .WithMessage("End date must be after start date.");
        }
    }
}
