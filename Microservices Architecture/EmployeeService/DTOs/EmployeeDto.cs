namespace EmployeeService.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
    }

    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
