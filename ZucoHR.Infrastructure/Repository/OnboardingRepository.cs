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
    public class OnboardingRepository : IOnboardingRepository
    {
        private readonly ZucoHrDbContext _context;

        public OnboardingRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<Onboarding?> GetByIdAsync(Guid orgId,Guid id)
        {
            return await _context.Onboardings
                .Where(x=>x.OrganizationId== orgId)
                .FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<List<Onboarding>> GetAllAsync(Guid orgId)
        {
            return await _context.Onboardings
                .AsNoTracking()
                .Where(x=>x.OrganizationId == orgId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Onboarding onboarding)
        {
            await _context.Onboardings.AddAsync(onboarding);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Onboarding onboarding)
        {
            _context.Onboardings.Update(onboarding);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskAsync(OnboardingTask task)
        {
            await _context.OnboardingTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OnboardingTask>> GetTasksAsync(Guid orgId,Guid onboardingId)
        {
            return await _context.OnboardingTasks
                .Where(x => x.OnboardingId == onboardingId && x.OrganizationId == orgId)
                .ToListAsync();
        }

        public async Task UpdateTaskAsync(OnboardingTask task)
        {
            _context.OnboardingTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task AddDocumentAsync(OnboardingDocument doc)
        {
            await _context.OnboardingDocuments.AddAsync(doc);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OnboardingDocument>> GetDocumentsAsync(Guid orgId, Guid onboardingId)
        {
            return await _context.OnboardingDocuments
                .Where(x => x.OnboardingId == onboardingId && x.OrganizationId ==orgId)
                .ToListAsync();
        }
    }
}
