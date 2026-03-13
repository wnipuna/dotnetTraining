using EFCoreConsoleApp.Models;

namespace EFCoreConsoleApp.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesWithDepartmentsAsync();
        Task<IEnumerable<Employee>> GetEmployeesWithProjectsAsync();
        Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<object>> GetEmployeesGroupedByDepartmentAsync();
        Task<IEnumerable<object>> GetEmployeeSalaryStatisticsByDepartmentAsync();
        Task<IEnumerable<object>> GetEmployeesWithProjectCountAsync();
        Task<IEnumerable<object>> GetTopEarnersByDepartmentAsync();
        Task<IEnumerable<object>> GetEmployeeProjectDetailsAsync();
        Task<IEnumerable<object>> GetDepartmentEmployeeCountAsync();
        Task<IEnumerable<Employee>> GetEmployeesHiredInLastMonthsAsync(int months);
        Task<IEnumerable<object>> GetComplexJoinQueryAsync();
    }
}
