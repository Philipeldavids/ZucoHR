using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Application.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IOnboardingRepository _repository;
        private readonly ITenantService _tenantService;

        public OnboardingService(IOnboardingRepository repository, ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
        }

        public async Task<List<Onboarding>> GetAll()
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetAllAsync(orgId);
        }

        public async Task<Onboarding?> Get(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetByIdAsync(orgId, id);
        }

        public async Task StartOnboarding(Guid applicantId)
        {
            var onboarding = new Onboarding
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenantService.GetTenantId(),
                ApplicantId = applicantId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(onboarding);
        }

        public async Task AddTask(Guid onboardingId, OnboardingTask task)
        {
            task.Id = Guid.NewGuid();
            task.OrganizationId = _tenantService.GetTenantId();
            task.OnboardingId = onboardingId;
            task.CreatedAt = DateTime.UtcNow;

            await _repository.AddTaskAsync(task);
        }

        public async Task CompleteTask(Guid taskId)
        {
            var orgId = _tenantService.GetTenantId();
            var tasks = await _repository.GetTasksAsync(orgId, Guid.Empty); // improve later
            var task = tasks.FirstOrDefault(x => x.Id == taskId);

            if (task == null)
                throw new Exception("Task not found");

            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;

            await _repository.UpdateTaskAsync(task);
        }

        public async Task UploadDocument(Guid onboardingId, OnboardingDocument doc)
        {
            doc.Id = Guid.NewGuid();
            doc.OrganizationId = _tenantService.GetTenantId();
            doc.OnboardingId = onboardingId;
            doc.UploadedAt = DateTime.UtcNow;

            await _repository.AddDocumentAsync(doc);
        }

        public async Task CompleteOnboarding(Guid onboardingId)
        {
            var orgId = _tenantService.GetTenantId();
            var onboarding = await _repository.GetByIdAsync(orgId, onboardingId);

            if (onboarding == null)
                throw new Exception("Onboarding not found");

            onboarding.Status = "Completed";
            onboarding.CompletedAt = DateTime.UtcNow;

            // 🔥 Here is where you can call EmployeeService
            // Create employee from applicant

            await _repository.UpdateAsync(onboarding);
        }
    }
}
