using BCrypt.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IUserRepository _userRepo;
        private readonly ITenantService _tenantService;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IEmailService _emailService;
        private readonly ZucoHrDbContext _context;
        public EmployeeService(ZucoHrDbContext context,IEmailService emailService, IEmployeeRepository repository, IUserRepository userRepo, IHubContext<NotificationHub> hub, ITenantService tenantService)
        {
            _repository = repository;
            _userRepo = userRepo;
            _tenantService = tenantService;
            _hub = hub;
            _context = context;
            _emailService = emailService;
        }

        public async Task<PaginatedResponse<Employee>> GetPagedAsync(int page, int pageSize, Guid orgId)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging parameters.");

            return await _repository.GetPagedAsync(page, pageSize, orgId);
        }

        public async Task<Employee?> GetByIdAsync(Guid id, Guid orgId)
        {
            return await _repository.GetByIdAsync(id, orgId);
        }

       
        public async Task<Employee> CreateAsync(string userId, EmployeeDto dto)
        {
            var orgId = _tenantService.GetTenantId();
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));


            //var user = await _userRepo.GetByIdAsync(dto.UserId, orgId);
            var password = PasswordGenerator.Generate(10);

            var user = await _context.Users
       .FirstOrDefaultAsync(x =>
           x.Id == userId
       );
            if(user != null)
            {
                Employee employee = new Employee();
                employee.Id = user.EmployeeId;
                employee.OrganizationId = orgId;
                employee.UserId = user.Id;
                employee.FirstName = dto.FirstName;
                employee.LastName = dto.LastName;
                employee.StartDate = dto.StartDate;
                employee.Email = dto.Email;
                employee.Position = dto.Position;
                employee.BasicSalary = dto.BasicSalary;
                employee.Allowance = dto.Allowance;
                employee.AnnualRent = dto.AnnualRent;
                employee.Department = dto.Department;
                employee.UpdatedAt = DateTime.UtcNow;
                employee.EmploymentType = dto.EmploymentType;
                employee.PhoneNumber = dto.PhoneNumber;
                //emp.UserId = Guid.NewGuid().ToString();        
                employee.Status = dto.Status;
                employee.Location = dto.Location;

                await _repository.AddAsync(employee);
            }

            User newuser = new User();
            
            newuser.UserName = dto.Email;
            newuser.Email = dto.Email;
            newuser.Name = dto.FirstName + " " + dto.LastName;
            newuser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            newuser.OrganizationId = orgId;
            newuser.EmployeeId = Guid.NewGuid();
            newuser.Role = "Employee";

            Employee emp = new Employee();
            emp.Id = newuser.EmployeeId;
            emp.OrganizationId = orgId;
            emp.UserId = newuser.Id;
            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.StartDate = dto.StartDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BasicSalary = dto.BasicSalary;
            emp.Allowance = dto.Allowance;
            emp.AnnualRent = dto.AnnualRent;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.EmploymentType = dto.EmploymentType;
            emp.PhoneNumber = dto.PhoneNumber;
            //emp.UserId = Guid.NewGuid().ToString();        
            emp.Status = dto.Status;
            emp.Location = dto.Location;
            //emp.Avatar = dto.Avatar;
           


            

            await _userRepo.AddAsync(newuser);
            await _repository.AddAsync(emp);
            await _emailService.SendEmailAsync(
    new EmailRequest
    {
        To = emp.Email,
        Subject = "Welcome to ZucoHR",
        Body = EmailTemplates.WelcomeEmployee(
            emp.FirstName,
            "ZucoHR",
            emp.Email,
            password
            
        )
    }
);
            return emp;
        }

        public async Task UpdateAsync(Guid id, EmployeeDto dto)
        {
            var orgId = _tenantService.GetTenantId();
            var emp = await _repository.GetByIdAsync(id, orgId);
            if (emp == null)
                throw new KeyNotFoundException("Employee not found");

            emp.OrganizationId = orgId;
            
            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.StartDate = dto.StartDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BasicSalary = dto.BasicSalary;
            emp.Allowance = dto.Allowance;
            emp.AnnualRent = dto.AnnualRent;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.PhoneNumber = dto.PhoneNumber;
            emp.EmployeeNumber = dto.EmployeeNumber;
            emp.EmploymentType = dto.EmploymentType;
            emp.Status = dto.Status;
            //emp.Avatar = dto.Avatar;
            emp.Location = dto.Location;

            

            await _repository.UpdateAsync(emp);
        }

        public async Task DeleteAsync(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var existing = await _repository.GetByIdAsync(id, orgId);
            if (existing == null)
                throw new KeyNotFoundException("Employee not found");

            await _repository.DeleteAsync(existing);
        }
    }

}
