using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIExtensionsDemo
{
    internal class EmployeeService : IEmployeeService
    {
        public async Task<List<Employee>> GetAllAsync()
        {
            await Task.Delay(100);
            return new List<Employee>
            {
                new(1, "john doe", 30, "IT"),
                new(2, "JANE SMITH", 25, "HR"),
                new(3, "alice", 42, "Finance"),
                new(4, "bob", 37, "IT"),
            };
        }
    }
}
