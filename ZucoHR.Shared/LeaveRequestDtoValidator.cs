using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Shared
{
    public class LeaveRequestDtoValidator : AbstractValidator<LeaveRequestDto>
    {
        public LeaveRequestDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required.")
                .MaximumLength(500)
                .WithMessage("Reason must not exceed 500 characters.");
        }
    }
}
