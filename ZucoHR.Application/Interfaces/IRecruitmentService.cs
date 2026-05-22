using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Application.Interfaces
{
    public interface IRecruitmentService
    {
        // Jobs
        Task<PaginatedResponse<JobPost>> GetJobs(int page = 1, int pageSize = 20);
        Task<JobPost?> GetJob(Guid id);
        Task CreateJob(JobPostRequest job);
        Task UpdateJob(Guid id, JobPostRequest job);
        Task DeleteJob(Guid id);
        Task CloseJobAsync(Guid id);

        // Applicants
        Task<PaginatedResponse<Applicant>> GetApplicants(int page = 1, int pageSize = 100);
        Task<List<Applicant>> GetApplicants(Guid jobId);
        Task ApplyAsync(Guid OrgId,
      Guid employeeId,
      ApplyJobRequest request);
        Task UpdateStatus(Guid applicantId, string status);
    }
}
