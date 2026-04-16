using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IRecruitmentService
    {
        // Jobs
        Task<List<JobPost>> GetJobs();
        Task<JobPost?> GetJob(Guid id);
        Task CreateJob(JobPost job);
        Task UpdateJob(Guid id, JobPost job);
        Task DeleteJob(Guid id);

        // Applicants
        Task<List<Applicant>> GetApplicants(Guid jobId);
        Task Apply(Guid jobId, Applicant applicant);
        Task UpdateStatus(Guid applicantId, ApplicationStatus status);
    }
}
