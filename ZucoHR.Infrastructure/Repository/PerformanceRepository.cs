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
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly ZucoHrDbContext _context;

        public PerformanceRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<List<PerformanceReview>> GetByEmployeeAsync(Guid orgId, Guid employeeId)
        {
            return await _context.PerformanceReviews
                .Where(x => x.EmployeeId == employeeId && x.OrganizationId == orgId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<PerformanceReview?> GetByIdAsync(Guid orgId, Guid id)
        {
            var reviewed = await _context.PerformanceReviews
                .Include(x=>x.Employee)
                .Include(x => x.Goals)
                .Include(x => x.Competencies)
                .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
            return reviewed;
        }

        public async Task CreateAsync(PerformanceReview review)
        {
            await _context.PerformanceReviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task Update(PerformanceReview review)
        {
           
        }

        public async Task DeleteAsync(PerformanceReview review)
        {
            _context.PerformanceReviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<PerformanceReview>> GetReviews(Guid orgId, int page = 1, int pageSize = 10)
        {
            var query = _context.PerformanceReviews.AsQueryable()
               .Where(x => x.OrganizationId == orgId)
               .Include(x => x.Employee)
                .Include(x => x.Competencies)
                .Include(x => x.Goals);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<PerformanceReview>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
