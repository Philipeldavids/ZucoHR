using Azure;
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
   
        public class RecruitmentRepository : IRecruitmentRepository
        {
            private readonly ZucoHrDbContext _context;

            public RecruitmentRepository(ZucoHrDbContext context)
            {
                _context = context;
            }

            // Jobs
            public async Task<PaginatedResponse<JobPost>> GetJobsAsync(Guid orgId, int page, int pageSize)
            {
            var query = _context.JobPosts.AsQueryable()
           .Where(x => x.OrganizationId == orgId);
          

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<JobPost>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

            public async Task<JobPost?> GetJobByIdAsync(Guid orgId, Guid id)
            {
                return await _context.JobPosts
                    .Where(x=>x.OrganizationId==orgId)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            public async Task AddJobAsync(JobPost job)
            {
                await _context.JobPosts.AddAsync(job);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateJobAsync(JobPost job)
            {
                _context.JobPosts.Update(job);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteJobAsync(JobPost job)
            {
                _context.JobPosts.Remove(job);
                await _context.SaveChangesAsync();
            }

            // Applicants

        public async Task<PaginatedResponse<Applicant>> GetApplicants(Guid orgId,int page, int pageSize)
        {
            var query = _context.Applicants.AsQueryable()
           .Where(x => x.OrganizationId == orgId);


            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.AppliedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Applicant>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
            public async Task<List<Applicant>> GetApplicantsByJobAsync(Guid orgId,Guid jobId)
            {
                return await _context.Applicants
                    .AsNoTracking()
                    .Where(x => x.JobPostId == jobId && x.OrganizationId == orgId)
                    .OrderByDescending(x => x.AppliedAt)
                    .ToListAsync();
            }

            public async Task<Applicant?> GetApplicantByIdAsync(Guid orgId, Guid id)
            {

                return await _context.Applicants
                    .Where(x=> x.OrganizationId==orgId)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            public async Task AddApplicantAsync(Applicant applicant)
            {
                await _context.Applicants.AddAsync(applicant);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateApplicantAsync(Applicant applicant)
            {
                _context.Applicants.Update(applicant);
                await _context.SaveChangesAsync();
            }
        }
    }
