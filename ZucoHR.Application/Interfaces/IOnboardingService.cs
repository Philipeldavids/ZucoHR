using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IOnboardingService
    {
        Task<List<Onboarding>> GetAll();
        Task<Onboarding?> Get(Guid id);

        Task StartOnboarding(Guid applicantId);

        Task AddTask(Guid onboardingId, OnboardingTask task);
        Task CompleteTask(Guid taskId);

        Task UploadDocument(Guid onboardingId, OnboardingDocument doc);

        Task CompleteOnboarding(Guid onboardingId);
    }
}
