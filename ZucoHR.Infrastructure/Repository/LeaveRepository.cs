using Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly ZucoHrDbContext _context;

        public LeaveRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> AddAsync(LeaveRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }
        public async Task<PaginatedResponse<LeaveRequest>> GetAll(Guid orgId, int page = 1, int pageSize = 10)
        {
            var query = _context.LeaveRequests.AsQueryable()
               .Where(x => x.OrganizationId == orgId)
               .Include(l => l.Employee);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<LeaveRequest>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<LeaveRequest?> GetByIdAsync(Guid id, Guid OrgId)
        {
            return await _context.LeaveRequests
                .AsNoTracking().
                Where(x=>x.OrganizationId == OrgId)
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId, Guid orgId)
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Where(l => l.EmployeeId == employeeId && l.OrganizationId == orgId)
                .Include(l=>l.Employee)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(LeaveRequest request)
        {
            var existing = await _context.LeaveRequests
                .FirstOrDefaultAsync(l => l.Id == request.Id);

            if (existing == null)
                throw new KeyNotFoundException("Leave request not found.");

            existing.StartDate = request.StartDate;
            existing.EndDate = request.EndDate;
            existing.Status = request.Status;
            existing.Reason = request.Reason;
            existing.ApprovedBy = request.ApprovedBy;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.LeaveRequests.Update(existing);
            await _context.SaveChangesAsync();
        }
    }
}
