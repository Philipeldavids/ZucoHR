using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Permission
    {
        public string Id { get; set; }
        public string Code { get; set; } // "PAYROLL_VIEW", "LEAVE_APPROVE"
        //public string Description { get; set; }

                //PAYROLL_VIEW
                //PAYROLL_RUN
                //EMPLOYEE_CREATE
                //LEAVE_APPROVE
                //EXPENSE_APPROVE
                //RECRUITMENT_MANAGE
    }
}
