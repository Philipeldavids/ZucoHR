using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IUserRepository _userRepo;
        private readonly ITenantService _tenantService;

        public EmployeeService(IEmployeeRepository repository, IUserRepository userRepo, ITenantService tenantService)
        {
            _repository = repository;
            _userRepo = userRepo;
            _tenantService = tenantService;
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, Guid orgId)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging parameters.");

            return await _repository.GetPagedAsync(page, pageSize, orgId);
        }

        public async Task<Employee?> GetByIdAsync(Guid id, Guid orgId)
        {
            return await _repository.GetByIdAsync(id, orgId);
        }

       
        public async Task<Employee> CreateAsync(EmployeeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepo.GetByIdAsync(dto.UserId);
            Employee emp = new Employee();
            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.HireDate = dto.HireDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BasicSalary = dto.BaseSalary;
            emp.Allowances = dto.Allowances;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.EmployeeNumber = dto.EmployeeNumber;
            emp.UserId = dto.UserId;
            emp.Id = user.EmployeeId;
            emp.OrganizationId = user.OrganizationId;

            await _repository.AddAsync(emp);
            return emp;
        }

        public async Task UpdateAsync(Guid id, EmployeeDto dto)
        {
            var orgId = _tenantService.GetTenantId();
            var emp = await _repository.GetByIdAsync(id, orgId);
            if (emp == null)
                throw new KeyNotFoundException("Employee not found");


            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.HireDate = dto.HireDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BasicSalary = dto.BaseSalary;
            emp.Allowances = dto.Allowances;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.EmployeeNumber = dto.EmployeeNumber;
            emp.UserId = dto.UserId;


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
