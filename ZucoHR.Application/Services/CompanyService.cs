using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ZucoHrDbContext _context;
        public CompanyService(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<PublicCompanyProfileDto>
    GetPublicCompanyProfileAsync(string slug)
        {
            var organization = await _context.Organizations.Where(x=>x.Slug == slug)
                .FirstOrDefaultAsync(x=>x.Slug == slug);

            if (organization == null)
                throw new Exception("Organization not found");

            return new PublicCompanyProfileDto
            {
                Name = organization.Name,
                Slug = organization.Slug

                //Id = organization.Id
                
            };
        }
    }
}
