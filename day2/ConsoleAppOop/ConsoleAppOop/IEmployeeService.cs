public interface IEmployeeService
{
    void AddEmployee(Employee employee);
    Employee GetEmployeeById(int id);
    List<Employee> GetAllEmployees();
}
