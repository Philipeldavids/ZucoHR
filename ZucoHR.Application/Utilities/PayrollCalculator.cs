using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Utilities
{
    public class PayrollCalculator
    {
        public static (decimal gross, decimal pension, decimal nhf, decimal tax, decimal net)
            CalculateMonthly(decimal basic, decimal allowances)
        {
            var monthlyGross = basic + allowances;

            var annualGross = monthlyGross * 12;

            // Pension (8%)
            var annualPension = annualGross * 0.08m;

            // NHF (2.5% of basic)
            var annualNhf = (basic * 12) * 0.025m;

            // CRA
            var cra = Math.Max(200000m, annualGross * 0.01m) + (annualGross * 0.20m);

            var taxable = annualGross - cra - annualPension - annualNhf;
            if (taxable < 0) taxable = 0;

            var annualTax = CalculateTax(taxable);

            // Convert back to monthly
            var tax = annualTax / 12;
            var pension = annualPension / 12;
            var nhf = annualNhf / 12;

            var net = monthlyGross - (tax + pension + nhf);

            return (monthlyGross, pension, nhf, tax, net);
        }

        private static decimal CalculateTax(decimal taxable)
        {
            decimal tax = 0;

            var bands = new (decimal limit, decimal rate)[]
            {
            (300000, 0.07m),
            (300000, 0.11m),
            (500000, 0.15m),
            (500000, 0.19m),
            (1600000, 0.21m),
            (decimal.MaxValue, 0.24m)
            };

            foreach (var (limit, rate) in bands)
            {
                var amount = Math.Min(taxable, limit);
                tax += amount * rate;
                taxable -= amount;

                if (taxable <= 0) break;
            }

            return tax;
        }
    }
}
