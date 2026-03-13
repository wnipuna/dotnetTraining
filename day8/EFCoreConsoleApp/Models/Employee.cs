namespace EFCoreConsoleApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }
}
