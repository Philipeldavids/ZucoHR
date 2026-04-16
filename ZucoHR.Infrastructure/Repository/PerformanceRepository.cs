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
            return await _context.PerformanceReviews
                .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        }

        public async Task CreateAsync(PerformanceReview review)
        {
            await _context.PerformanceReviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PerformanceReview review)
        {
            _context.PerformanceReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PerformanceReview review)
        {
            _context.PerformanceReviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
