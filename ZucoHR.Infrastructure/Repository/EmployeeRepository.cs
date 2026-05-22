using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ZucoHrDbContext _context;
        public EmployeeRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<Employee>> GetPagedAsync(int page, int pageSize, Guid orgId)
        {
            var query = _context.Employees.AsQueryable()
                .Where(x=> x.OrganizationId == orgId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Employee>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Employee?> GetByIdAsync(Guid id,Guid orgId) => await _context.Employees.Where(x=>x.OrganizationId==orgId).FirstOrDefaultAsync(x=>x.Id==id);

        public async Task AddAsync(Employee e)
        {
            
            await _context.Employees.AddAsync(e);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee e)
        {         

            _context.Employees.Update(e);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee e)
        {
            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();
        }
    }









}
