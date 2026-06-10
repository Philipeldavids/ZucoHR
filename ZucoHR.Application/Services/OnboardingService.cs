using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly ZucoHrDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;

        public OnboardingService(
            IEmailService emailService,
            ZucoHrDbContext context,
            ITenantService tenantService
        )
        {
            _context = context;
            _tenantService = tenantService;
            _emailService = emailService;
        }

        public async Task<PaginatedResponse<OnboardingTaskResponseDTO>>
            GetAllAsync(Guid orgId, int page, int pageSize)
        {
            var query = _context.OnboardingTasks.AsQueryable()
                .Where(x => x.OrganizationId == orgId)
                .Include(x => x.Employee);
                

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Select(x => new OnboardingTaskResponseDTO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    EmployeeName =
                        x.Employee!.FirstName +
                        " " +
                        x.Employee.LastName,
                    Title = x.Title,
                    Description = x.Description,
                    Category = x.Category,
                    Status = x.Status,
                    DueDate = x.DueDate,
                    CompletedAt = x.CompletedAt,
                    CreatedAt = x.CreatedAt
                })               
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();



            return new PaginatedResponse<OnboardingTaskResponseDTO>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize

            };
        }

        public async Task<OnboardingTaskResponseDTO?>
            GetByIdAsync(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _context.OnboardingTasks
                .Include(x => x.Employee)
                .Where(x => x.Id == id && x.OrganizationId == orgId)
                .Select(x => new OnboardingTaskResponseDTO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    EmployeeName =
                        x.Employee!.FirstName +
                        " " +
                        x.Employee.LastName,
                    Title = x.Title,
                    Description = x.Description,
                    Category = x.Category,
                    Status = x.Status,
                    DueDate = x.DueDate,
                    CompletedAt = x.CompletedAt,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<OnboardingTaskResponseDTO>
            CreateAsync(
                CreateOnboardingTaskDTO dto
            )
        {
            var orgId = _tenantService.GetTenantId();

            var emp =await  _context.Employees.FirstOrDefaultAsync(x => x.Id == dto.EmployeeId);
            var task = new OnboardingTask
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                Employee = emp,
                AssignedTo = dto.EmployeeId,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Status = "pending",
                OrganizationId = orgId,
                DueDate = dto.DueDate.ToUniversalTime()
            };

            _context.OnboardingTasks.Add(task);

            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(
    new EmailRequest
    {
        To = task.Employee.Email,
        Subject = "Task Assigned",
        Body = EmailTemplates.TaskAssigned(
            task.Employee.FirstName,
            task.Title,
            task.DueDate
        )
    }
);
            return await GetByIdAsync(task.Id)
                ?? throw new Exception("Task not found");
        }

        public async Task<bool> UpdateAsync(
            Guid id,
            UpdateOnboardingTaskDTO dto
        )
        {
            var task =
                await _context.OnboardingTasks
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                task.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Category))
                task.Category = dto.Category;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                task.Status = dto.Status;

            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate.Value.ToUniversalTime();

            task.CompletedAt = dto.CompletedAt;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task =
                await _context.OnboardingTasks
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return false;

            _context.OnboardingTasks.Remove(task);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
