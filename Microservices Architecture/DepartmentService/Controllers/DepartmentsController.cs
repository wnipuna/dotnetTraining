using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DepartmentService.Data;
using DepartmentService.Models;
using DepartmentService.DTOs;

namespace DepartmentService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentDbContext _context;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(DepartmentDbContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            _logger.LogInformation("Getting all departments (v1.0)");
            var departments = await _context.Departments
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Location = d.Location
                })
                .ToListAsync();

            return Ok(departments);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<object>> GetDepartmentsV2()
        {
            _logger.LogInformation("Getting all departments (v2.0) with metadata");
            var departments = await _context.Departments
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Location = d.Location
                })
                .ToListAsync();

            return Ok(new
            {
                Version = "2.0",
                Count = departments.Count,
                Data = departments
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            _logger.LogInformation("Getting department with ID: {Id}", id);
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                _logger.LogWarning("Department with ID {Id} not found", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            var departmentDto = new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Location = department.Location
            };

            return Ok(departmentDto);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto createDto)
        {
            _logger.LogInformation("Creating new department: {Name}", createDto.Name);
            
            var department = new Department
            {
                Name = createDto.Name,
                Location = createDto.Location
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var departmentDto = new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Location = department.Location
            };

            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, departmentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto updateDto)
        {
            _logger.LogInformation("Updating department with ID: {Id}", id);
            
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {Id} not found", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            department.Name = updateDto.Name;
            department.Location = updateDto.Location;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            _logger.LogInformation("Deleting department with ID: {Id}", id);
            
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {Id} not found", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
