using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IRecruitmentRepository
    {
        // Jobs
        Task<PaginatedResponse<JobPost>> GetJobsAsync(Guid orgId, int page, int pageSize);
        Task<JobPost?> GetJobByIdAsync(Guid orgId,Guid id);
        Task AddJobAsync(JobPost job);
        Task UpdateJobAsync(JobPost job);
        Task DeleteJobAsync(JobPost job);

        // Applicants
        Task<PaginatedResponse<Applicant>> GetApplicants(Guid orgId, int page, int pageSize);
        Task<List<Applicant>> GetApplicantsByJobAsync(Guid orgId, Guid jobId);
        Task<Applicant?> GetApplicantByIdAsync(Guid orgId, Guid id);
        Task AddApplicantAsync(Applicant applicant);
        Task UpdateApplicantAsync(Applicant applicant);
    }
}
