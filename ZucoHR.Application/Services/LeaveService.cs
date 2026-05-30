using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository; // optional, for validation
        private readonly ILogger<LeaveService> _logger;
        private readonly ITenantService _tenantService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        public LeaveService(
            IEmailService emailService,
            ILeaveRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            ILogger<LeaveService> logger,
            INotificationService service,
            ITenantService tenantService)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _tenantService = tenantService;
            _notificationService = service;
            _emailService = emailService;
        }
        public async Task<PaginatedResponse<LeaveRequest>> GetAll(Guid orgId, int page, int pagesize)
        {
            return await _leaveRepository.GetAll(orgId, page, pagesize);
        }
        public async Task<IEnumerable<LeaveRequest>> GetByEmployee(Guid employeeId)
        {
            var orgID = _tenantService.GetTenantId();

            var req = await _leaveRepository.GetByEmployeeAsync(employeeId, orgID);
            return req;        
        }
        public async Task<LeaveRequest> RequestLeaveAsync(Guid employeeId,string leaveType, DateTime start, DateTime end, string reason)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be earlier than end date.");
            var orgId = _tenantService.GetTenantId();
            var employee = await _employeeRepository.GetByIdAsync(employeeId, orgId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");
            var days = end - start;

            var leave = new LeaveRequest
            {
                EmployeeId = employeeId,
                Employee = employee,
                Type = leaveType,
                StartDate = start,
                EndDate = end,
                Days = days.Days + 1,
                Reason = reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId
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

            if (leave.Status != "Pending")
                throw new InvalidOperationException("Only pending requests can be approved.");

            leave.Status = "Approved";
            leave.ApprovedBy = approverId;
            leave.UpdatedAt = DateTime.UtcNow;

            var employeeId = leave.EmployeeId;
            await _leaveRepository.UpdateAsync(leave);
            _logger.LogInformation("Leave {LeaveId} approved by {ApproverId}", leaveId, approverId);
            await _notificationService.CreateAsync(
    employeeId.ToString(),
    "Leave Approved",
    $"Your leave from {leave.StartDate:d} to {leave.EndDate:d} has been approved"
);
            await _emailService.SendEmailAsync(
    new EmailRequest
    {
        To = leave.Employee.Email,
        Subject = "Leave Approved",
        Body = EmailTemplates.LeaveApproved(
            leave.Employee.FirstName,
            leave.Type,
            leave.Days
        )
    }
);
        }

        public async Task RejectAsync(Guid leaveId, Guid approverId, string comment)
        {
            var orgId = _tenantService.GetTenantId();
            var leave = await _leaveRepository.GetByIdAsync(leaveId, orgId);
            if (leave == null)
                throw new KeyNotFoundException("Leave request not found.");

            if (leave.Status != "Pending")
                throw new InvalidOperationException("Only pending requests can be rejected.");

            leave.Status = "Rejected";
            leave.ApprovedBy = approverId;
            leave.Reason += $"\n[Manager Comment]: {comment}";
            leave.UpdatedAt = DateTime.UtcNow;

            await _leaveRepository.UpdateAsync(leave);
            _logger.LogInformation("Leave {LeaveId} rejected by {ApproverId}. Comment: {Comment}", leaveId, approverId, comment);
            await _emailService.SendEmailAsync(
        new EmailRequest
        {
            To = leave.Employee.Email,
            Subject = "Task Assigned",
            Body = EmailTemplates.LeaveRejected(
                leave.Employee.FirstName,
                leave.Type
            )
        }
    );
        }
    }
}
