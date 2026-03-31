namespace ProjectService.DTOs
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<EmployeeAssignmentDto>? Assignments { get; set; }
    }

    public class CreateProjectDto
    {
        public string ProjectName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateProjectDto
    {
        public string ProjectName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? EndDate { get; set; }
    }

    public class EmployeeAssignmentDto
    {
        public int EmployeeProjectId { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Role { get; set; } = null!;
        public EmployeeDto? Employee { get; set; }
    }

    public class CreateEmployeeAssignmentDto
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Role { get; set; } = null!;
    }

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
