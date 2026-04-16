using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IOnboardingRepository
    {
        Task<Onboarding?> GetByIdAsync(Guid orgId,Guid id);
        Task<List<Onboarding>> GetAllAsync(Guid orgId);

        Task AddAsync(Onboarding onboarding);
        Task UpdateAsync(Onboarding onboarding);

        Task AddTaskAsync(OnboardingTask task);
        Task<List<OnboardingTask>> GetTasksAsync(Guid orgId, Guid onboardingId);
        Task UpdateTaskAsync(OnboardingTask task);

        Task AddDocumentAsync(OnboardingDocument doc);
        Task<List<OnboardingDocument>> GetDocumentsAsync(Guid orgId, Guid onboardingId);
    }
}
