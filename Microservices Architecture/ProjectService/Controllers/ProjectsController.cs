using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.Models;
using ProjectService.DTOs;
using ProjectService.Services;

namespace ProjectService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectDbContext _context;
        private readonly EmployeeServiceClient _employeeClient;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            ProjectDbContext context,
            EmployeeServiceClient employeeClient,
            ILogger<ProjectsController> logger)
        {
            _context = context;
            _employeeClient = employeeClient;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            _logger.LogInformation("Getting all projects (v1.0)");
            var projects = await _context.Projects
                .Select(p => new ProjectDto
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsV2()
        {
            _logger.LogInformation("Getting all projects with assignments (v2.0)");
            var projects = await _context.Projects
                .Include(p => _context.EmployeeProjects.Where(ep => ep.ProjectId == p.ProjectId))
                .Select(p => new ProjectDto
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Assignments = _context.EmployeeProjects
                        .Where(ep => ep.ProjectId == p.ProjectId)
                        .Select(ep => new EmployeeAssignmentDto
                        {
                            EmployeeProjectId = ep.EmployeeProjectId,
                            EmployeeId = ep.EmployeeId,
                            ProjectId = ep.ProjectId,
                            AssignedDate = ep.AssignedDate,
                            Role = ep.Role
                        }).ToList()
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            _logger.LogInformation("Getting project with ID: {Id}", id);
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with ID {Id} not found", id);
                return NotFound(new { message = $"Project with ID {id} not found" });
            }

            var assignments = await _context.EmployeeProjects
                .Where(ep => ep.ProjectId == id)
                .ToListAsync();

            var assignmentDtos = new List<EmployeeAssignmentDto>();
            foreach (var assignment in assignments)
            {
                var employee = await _employeeClient.GetEmployeeAsync(assignment.EmployeeId);
                assignmentDtos.Add(new EmployeeAssignmentDto
                {
                    EmployeeProjectId = assignment.EmployeeProjectId,
                    EmployeeId = assignment.EmployeeId,
                    ProjectId = assignment.ProjectId,
                    AssignedDate = assignment.AssignedDate,
                    Role = assignment.Role,
                    Employee = employee
                });
            }

            var projectDto = new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Assignments = assignmentDtos
            };

            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createDto)
        {
            _logger.LogInformation("Creating new project: {Name}", createDto.ProjectName);
            
            var project = new Project
            {
                ProjectName = createDto.ProjectName,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectDto = new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };

            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, projectDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto updateDto)
        {
            _logger.LogInformation("Updating project with ID: {Id}", id);
            
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                _logger.LogWarning("Project with ID {Id} not found", id);
                return NotFound(new { message = $"Project with ID {id} not found" });
            }

            project.ProjectName = updateDto.ProjectName;
            project.Description = updateDto.Description;
            project.EndDate = updateDto.EndDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            _logger.LogInformation("Deleting project with ID: {Id}", id);
            
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                _logger.LogWarning("Project with ID {Id} not found", id);
                return NotFound(new { message = $"Project with ID {id} not found" });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("assignments")]
        public async Task<ActionResult<EmployeeAssignmentDto>> AssignEmployee(CreateEmployeeAssignmentDto createDto)
        {
            _logger.LogInformation("Assigning employee {EmployeeId} to project {ProjectId}", 
                createDto.EmployeeId, createDto.ProjectId);

            var project = await _context.Projects.FindAsync(createDto.ProjectId);
            if (project == null)
            {
                return NotFound(new { message = $"Project with ID {createDto.ProjectId} not found" });
            }

            var employee = await _employeeClient.GetEmployeeAsync(createDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest(new { message = $"Employee with ID {createDto.EmployeeId} not found" });
            }

            var assignment = new EmployeeProject
            {
                EmployeeId = createDto.EmployeeId,
                ProjectId = createDto.ProjectId,
                AssignedDate = createDto.AssignedDate,
                Role = createDto.Role
            };

            _context.EmployeeProjects.Add(assignment);
            await _context.SaveChangesAsync();

            var assignmentDto = new EmployeeAssignmentDto
            {
                EmployeeProjectId = assignment.EmployeeProjectId,
                EmployeeId = assignment.EmployeeId,
                ProjectId = assignment.ProjectId,
                AssignedDate = assignment.AssignedDate,
                Role = assignment.Role,
                Employee = employee
            };

            return CreatedAtAction(nameof(GetProject), new { id = createDto.ProjectId }, assignmentDto);
        }

        [HttpDelete("assignments/{id}")]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            _logger.LogInformation("Removing assignment with ID: {Id}", id);
            
            var assignment = await _context.EmployeeProjects.FindAsync(id);
            if (assignment == null)
            {
                _logger.LogWarning("Assignment with ID {Id} not found", id);
                return NotFound(new { message = $"Assignment with ID {id} not found" });
            }

            _context.EmployeeProjects.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
