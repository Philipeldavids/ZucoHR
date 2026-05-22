using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Infrastructure.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ZucoHrDbContext _context;


        public OrganizationRepository(ZucoHrDbContext context)
        {
            _context = context;   
        }
        public async Task<List<Organization>> GetOrganization()
        {
            return await _context.Organizations.ToListAsync();

        }
        public async Task<Organization> GetOrganizationByid(Guid Id)
        {
           var org = await _context.Organizations.Where(x => x.Id == Id).FirstOrDefaultAsync();
            return org;
        }
    }
}
