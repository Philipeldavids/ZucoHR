using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Utilities
{
    public class PayrollCalculator
    {
        public static (decimal gross, decimal pension, decimal nhf, decimal nhis, decimal tax, decimal rr, decimal net)
            CalculateMonthly(decimal basic, decimal allowances, decimal annualRent)
        {
            var monthlyGross = basic + allowances;

            var annualGross = monthlyGross * 12;

            // Pension (8%)
            var annualPension = annualGross * 0.08m;

            // NHF (2.5% of basic)
            var annualNhf = (basic * 12) * 0.025m;

            // CRA
            var annualrr = Math.Min(500000m, annualRent * 0.2m);
            //NHIS

            var annualNhis = (basic * 12) * 0.05m;

            var taxable = annualGross - annualrr - annualPension - annualNhf - annualNhis;
            if (taxable < 0) taxable = 0;

            var annualTax = CalculateTax(taxable);

            // Convert back to monthly
            var tax = annualTax / 12;
            var pension = annualPension / 12;
            var nhf = annualNhf / 12;
            var nhis = annualNhis / 12;
            var rr = annualrr / 12;
            var net = monthlyGross - (tax + pension + nhf + nhis);

            return (monthlyGross, pension, nhf, nhis, tax, rr, net);
        }

        private static decimal CalculateTax(decimal taxable)
        {
            decimal tax = 0;

            if (taxable <= 800000)
            {
                tax = 0;
            }
            else if (taxable <= 3000000)
            {
                tax = (taxable - 800000) * 0.15m;
            }
            else if (taxable <= 12000000)
            {
                tax = (2200000 * 0.15m) +
                      ((taxable - 3000000) * 0.18m);
            }
            else if (taxable <= 25000000)
            {
                tax = (2200000 * 0.15m) +
                      (9000000 * 0.18m) +
                      ((taxable - 12000000) * 0.21m);
            }
            else if (taxable <= 50000000)
            {
                tax = (2200000 * 0.15m) +
                      (9000000 * 0.18m) +
                      (13000000 * 0.21m) +
                      ((taxable - 25000000) * 0.23m);
            }
            else
            {
                tax = (2200000 * 0.15m) +
                      (9000000 * 0.18m) +
                      (13000000 * 0.21m) +
                      (25000000 * 0.23m) +
                      ((taxable - 50000000) * 0.25m);
            }

            return tax;
        }
    }
    }
