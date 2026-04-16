using Microsoft.Extensions.Logging;
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
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository; // optional, for validation
        private readonly ILogger<LeaveService> _logger;
        private readonly ITenantService _tenantService;
        public LeaveService(
            ILeaveRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            ILogger<LeaveService> logger,
            ITenantService tenantService)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _tenantService = tenantService;
        }

        public async Task<LeaveRequest> RequestLeaveAsync(Guid employeeId, DateTime start, DateTime end, string reason)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be earlier than end date.");
            var orgId = _tenantService.GetTenantId();
            var employee = await _employeeRepository.GetByIdAsync(employeeId, orgId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var leave = new LeaveRequest
            {
                EmployeeId = employeeId,
                StartDate = start,
                EndDate = end,
                Reason = reason,
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _leaveRepository.AddAsync(leave);
            _logger.LogInformation("Leave requested by {EmployeeId} from {Start} to {End}", employeeId, start, end);
            return leave;
        }

        public async Task ApproveAsync(Guid leaveId, Guid approverId)
        {
            var orgId = _tenantService.GetTenantId();
            var leave = await _leaveRepository.GetByIdAsync(leaveId, orgId);
            if (leave == null)
                throw new KeyNotFoundException("Leave request not found.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved.");

            leave.Status = LeaveStatus.Approved;
            leave.ApprovedBy = approverId;
            leave.UpdatedAt = DateTime.UtcNow;

            await _leaveRepository.UpdateAsync(leave);
            _logger.LogInformation("Leave {LeaveId} approved by {ApproverId}", leaveId, approverId);
        }

        public async Task RejectAsync(Guid leaveId, Guid approverId, string comment)
        {
            var orgId = _tenantService.GetTenantId();
            var leave = await _leaveRepository.GetByIdAsync(leaveId, orgId);
            if (leave == null)
                throw new KeyNotFoundException("Leave request not found.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");

            leave.Status = LeaveStatus.Rejected;
            leave.ApprovedBy = approverId;
            leave.Reason += $"\n[Manager Comment]: {comment}";
            leave.UpdatedAt = DateTime.UtcNow;

            await _leaveRepository.UpdateAsync(leave);
            _logger.LogInformation("Leave {LeaveId} rejected by {ApproverId}. Comment: {Comment}", leaveId, approverId, comment);
        }
    }
}
