using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Shared
{
   
        public class CreateEmployeeValidator : AbstractValidator<EmployeeDto>
        {
            public CreateEmployeeValidator()
            {
                RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
                RuleFor(x => x.BaseSalary).GreaterThan(0);
            }
        }
    
}
