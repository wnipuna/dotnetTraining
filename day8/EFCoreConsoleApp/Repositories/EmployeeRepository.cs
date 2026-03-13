using EFCoreConsoleApp.Data;
using EFCoreConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreConsoleApp.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithDepartmentsAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .OrderBy(e => e.Department.Name)
                .ThenBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithProjectsAsync()
        {
            return await _context.Employees
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            return await _context.Employees
                .Where(e => e.Salary >= minSalary && e.Salary <= maxSalary)
                .Include(e => e.Department)
                .OrderByDescending(e => e.Salary)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .Include(e => e.Department)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetEmployeesGroupedByDepartmentAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .GroupBy(e => new { e.Department.DepartmentId, e.Department.Name })
                .Select(g => new
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.Name,
                    EmployeeCount = g.Count(),
                    Employees = g.Select(e => new
                    {
                        e.EmployeeId,
                        e.Name,
                        e.Email,
                        e.Salary
                    }).ToList()
                })
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetEmployeeSalaryStatisticsByDepartmentAsync()
        {
            return await _context.Employees
                .GroupBy(e => new { e.DepartmentId, e.Department.Name })
                .Select(g => new
                {
                    DepartmentName = g.Key.Name,
                    EmployeeCount = g.Count(),
                    AverageSalary = g.Average(e => e.Salary),
                    MinSalary = g.Min(e => e.Salary),
                    MaxSalary = g.Max(e => e.Salary),
                    TotalSalary = g.Sum(e => e.Salary)
                })
                .OrderByDescending(x => x.AverageSalary)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetEmployeesWithProjectCountAsync()
        {
            return await _context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Name,
                    e.Email,
                    DepartmentName = e.Department.Name,
                    ProjectCount = e.EmployeeProjects.Count(),
                    Projects = e.EmployeeProjects.Select(ep => new
                    {
                        ep.Project.ProjectName,
                        ep.Role,
                        ep.AssignedDate
                    }).ToList()
                })
                .OrderByDescending(x => x.ProjectCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopEarnersByDepartmentAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();

            var topEarners = employees
                .GroupBy(e => e.DepartmentId)
                .Select(g => g.OrderByDescending(e => e.Salary).First())
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Name,
                    e.Salary,
                    DepartmentName = e.Department.Name,
                    DepartmentLocation = e.Department.Location
                })
                .ToList();

            return topEarners;
        }

        public async Task<IEnumerable<object>> GetEmployeeProjectDetailsAsync()
        {
            return await (from emp in _context.Employees
                         join empProj in _context.EmployeeProjects on emp.EmployeeId equals empProj.EmployeeId
                         join proj in _context.Projects on empProj.ProjectId equals proj.ProjectId
                         join dept in _context.Departments on emp.DepartmentId equals dept.DepartmentId
                         select new
                         {
                             EmployeeName = emp.Name,
                             EmployeeEmail = emp.Email,
                             DepartmentName = dept.Name,
                             ProjectName = proj.ProjectName,
                             ProjectDescription = proj.Description,
                             Role = empProj.Role,
                             AssignedDate = empProj.AssignedDate,
                             ProjectStartDate = proj.StartDate
                         })
                         .OrderBy(x => x.EmployeeName)
                         .ThenBy(x => x.ProjectName)
                         .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetDepartmentEmployeeCountAsync()
        {
            return await (from dept in _context.Departments
                         join emp in _context.Employees on dept.DepartmentId equals emp.DepartmentId into empGroup
                         select new
                         {
                             DepartmentId = dept.DepartmentId,
                             DepartmentName = dept.Name,
                             Location = dept.Location,
                             EmployeeCount = empGroup.Count(),
                             TotalSalaryExpense = empGroup.Sum(e => e.Salary),
                             AverageSalary = empGroup.Any() ? empGroup.Average(e => e.Salary) : 0
                         })
                         .OrderByDescending(x => x.EmployeeCount)
                         .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesHiredInLastMonthsAsync(int months)
        {
            var cutoffDate = DateTime.Now.AddMonths(-months);
            
            return await _context.Employees
                .Where(e => e.HireDate >= cutoffDate)
                .Include(e => e.Department)
                .OrderByDescending(e => e.HireDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetComplexJoinQueryAsync()
        {
            return await (from emp in _context.Employees
                         join dept in _context.Departments on emp.DepartmentId equals dept.DepartmentId
                         join empProj in _context.EmployeeProjects on emp.EmployeeId equals empProj.EmployeeId into empProjGroup
                         from ep in empProjGroup.DefaultIfEmpty()
                         join proj in _context.Projects on ep.ProjectId equals proj.ProjectId into projGroup
                         from p in projGroup.DefaultIfEmpty()
                         group new { emp, dept, ep, p } by new
                         {
                             emp.EmployeeId,
                             emp.Name,
                             emp.Email,
                             emp.Salary,
                             emp.HireDate,
                             DepartmentName = dept.Name,
                             DepartmentLocation = dept.Location
                         } into grouped
                         select new
                         {
                             grouped.Key.EmployeeId,
                             grouped.Key.Name,
                             grouped.Key.Email,
                             grouped.Key.Salary,
                             grouped.Key.HireDate,
                             grouped.Key.DepartmentName,
                             grouped.Key.DepartmentLocation,
                             ProjectCount = grouped.Count(g => g.p != null),
                             Projects = grouped.Where(g => g.p != null)
                                               .Select(g => new
                                               {
                                                   ProjectName = g.p.ProjectName,
                                                   Role = g.ep.Role,
                                                   AssignedDate = g.ep.AssignedDate
                                               })
                                               .Distinct()
                                               .ToList()
                         })
                         .OrderByDescending(x => x.ProjectCount)
                         .ThenBy(x => x.Name)
                         .ToListAsync();
        }
    }
}
