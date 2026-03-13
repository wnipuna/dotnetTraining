using Microsoft.AspNetCore.Mvc;
using EmployeeWebAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", Age = 30, Salary = 60000 },
            new Employee { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Department = "HR", Age = 28, Salary = 55000 },
            new Employee { Id = 3, Name = "Bob Johnson", Email = "bob.johnson@example.com", Department = "Finance", Age = 35, Salary = 70000 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAllEmployees()
        {
            try
            {
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving employees", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> GetEmployeeById(int id)
        {
            try
            {
                var employee = employees.FirstOrDefault(e => e.Id == id);
                
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found" });
                }
                
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the employee", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                employee.Id = employees.Any() ? employees.Max(e => e.Id) + 1 : 1;
                employees.Add(employee);
                
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the employee", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Employee> UpdateEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var employee = employees.FirstOrDefault(e => e.Id == id);
                
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found" });
                }

                employee.Name = updatedEmployee.Name;
                employee.Email = updatedEmployee.Email;
                employee.Department = updatedEmployee.Department;
                employee.Age = updatedEmployee.Age;
                employee.Salary = updatedEmployee.Salary;
                
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the employee", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                var employee = employees.FirstOrDefault(e => e.Id == id);
                
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found" });
                }

                employees.Remove(employee);
                
                return Ok(new { message = $"Employee with ID {id} has been deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the employee", error = ex.Message });
            }
        }
    }
}
