public class EmployeeService : IEmployeeService
{
    private List<Employee> Employees = new List<Employee>();

    public void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
    }

    public Employee GetEmployeeById(int id)
    {
        return Employees.FirstOrDefault(e => e.GetId == id);
    }

    public List<Employee> GetAllEmployees()
    {
        return Employees;
    }
}