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

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging parameters.");

            return await _repository.GetPagedAsync(page, pageSize);
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        
        public async Task<Employee> CreateAsync(EmployeeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            Employee emp = new Employee();
            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.HireDate = dto.HireDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BaseSalary = dto.BaseSalary;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.EmployeeNumber = dto.EmployeeNumber;
            emp.UserId = dto.UserId;

            await _repository.AddAsync(emp);
            return emp;
        }

        public async Task UpdateAsync(Guid id, EmployeeDto dto)
        {
            var emp = await _repository.GetByIdAsync(id);
            if (emp == null)
                throw new KeyNotFoundException("Employee not found");


            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.HireDate = dto.HireDate;
            emp.Email = dto.Email;
            emp.Position = dto.Position;
            emp.BaseSalary = dto.BaseSalary;
            emp.Department = dto.Department;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.EmployeeNumber = dto.EmployeeNumber;
            emp.UserId = dto.UserId;


            await _repository.UpdateAsync(emp);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Employee not found");

            await _repository.DeleteAsync(existing);
        }
    }

}
