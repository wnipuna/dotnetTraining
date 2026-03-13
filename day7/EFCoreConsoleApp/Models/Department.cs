namespace EFCoreConsoleApp.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
