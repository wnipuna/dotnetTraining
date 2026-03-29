namespace DepartmentService.DTOs
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    public class CreateDepartmentDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    public class UpdateDepartmentDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
