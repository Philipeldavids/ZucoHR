using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IRecruitmentRepository
    {
        // Jobs
        Task<List<JobPost>> GetJobsAsync(Guid orgId);
        Task<JobPost?> GetJobByIdAsync(Guid orgId,Guid id);
        Task AddJobAsync(JobPost job);
        Task UpdateJobAsync(JobPost job);
        Task DeleteJobAsync(JobPost job);

        // Applicants
        Task<List<Applicant>> GetApplicantsByJobAsync(Guid orgId, Guid jobId);
        Task<Applicant?> GetApplicantByIdAsync(Guid orgId, Guid id);
        Task AddApplicantAsync(Applicant applicant);
        Task UpdateApplicantAsync(Applicant applicant);
    }
}
