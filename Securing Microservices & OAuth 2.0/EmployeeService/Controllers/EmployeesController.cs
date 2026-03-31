using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EmployeeService.Data;
using EmployeeService.Models;
using EmployeeService.DTOs;
using EmployeeService.Services;

namespace EmployeeService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        private readonly DepartmentServiceClient _departmentClient;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            EmployeeDbContext context, 
            DepartmentServiceClient departmentClient,
            ILogger<EmployeesController> logger)
        {
            _context = context;
            _departmentClient = departmentClient;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            _logger.LogInformation("Getting all employees (v1.0)");
            var employees = await _context.Employees
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesV2()
        {
            _logger.LogInformation("Getting all employees with department info (v2.0)");
            var employees = await _context.Employees
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId
                })
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.Department = await _departmentClient.GetDepartmentAsync(employee.DepartmentId);
            }

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            _logger.LogInformation("Getting employee with ID: {Id}", id);
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                Department = await _departmentClient.GetDepartmentAsync(employee.DepartmentId)
            };

            return Ok(employeeDto);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(int departmentId)
        {
            _logger.LogInformation("Getting employees for department ID: {DepartmentId}", departmentId);
            var employees = await _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Email = e.Email,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    DepartmentId = e.DepartmentId
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createDto)
        {
            _logger.LogInformation("Creating new employee: {Name}", createDto.Name);
            
            var department = await _departmentClient.GetDepartmentAsync(createDto.DepartmentId);
            if (department == null)
            {
                return BadRequest(new { message = $"Department with ID {createDto.DepartmentId} not found" });
            }

            var employee = new Employee
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Salary = createDto.Salary,
                HireDate = createDto.HireDate,
                DepartmentId = createDto.DepartmentId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                DepartmentId = employee.DepartmentId,
                Department = department
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employeeDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateDto)
        {
            _logger.LogInformation("Updating employee with ID: {Id}", id);
            
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            var department = await _departmentClient.GetDepartmentAsync(updateDto.DepartmentId);
            if (department == null)
            {
                return BadRequest(new { message = $"Department with ID {updateDto.DepartmentId} not found" });
            }

            employee.Name = updateDto.Name;
            employee.Email = updateDto.Email;
            employee.Salary = updateDto.Salary;
            employee.DepartmentId = updateDto.DepartmentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {Id}", id);
            
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
