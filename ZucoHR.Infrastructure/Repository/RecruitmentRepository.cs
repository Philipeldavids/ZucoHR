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
   
        public class RecruitmentRepository : IRecruitmentRepository
        {
            private readonly ZucoHrDbContext _context;

            public RecruitmentRepository(ZucoHrDbContext context)
            {
                _context = context;
            }

            // Jobs
            public async Task<List<JobPost>> GetJobsAsync(Guid orgId)
            {
                return await _context.JobPosts
                    .AsNoTracking()
                    .Where(x=>x.OrganizationId == orgId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
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
