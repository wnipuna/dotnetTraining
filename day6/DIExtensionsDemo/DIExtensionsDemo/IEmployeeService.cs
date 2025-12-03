using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIExtensionsDemo
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
    }
}
