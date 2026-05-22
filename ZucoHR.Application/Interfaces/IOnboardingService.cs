using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IOnboardingService
    {
        Task<PaginatedResponse<OnboardingTaskResponseDTO>> GetAllAsync(Guid orgId, int page, int pageSize);

        Task<OnboardingTaskResponseDTO?> GetByIdAsync(Guid id);

        Task<OnboardingTaskResponseDTO> CreateAsync(
            CreateOnboardingTaskDTO dto
        );

        Task<bool> UpdateAsync(
            Guid id,
            UpdateOnboardingTaskDTO dto
        );

        Task<bool> DeleteAsync(Guid id);
    }
}
