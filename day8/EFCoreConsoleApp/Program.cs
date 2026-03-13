using EFCoreConsoleApp.Data;
using EFCoreConsoleApp.Models;
using EFCoreConsoleApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("=== EF Core Console Application with Repository Pattern ===\n");

using (var context = new AppDbContext())
{
    Console.WriteLine("Applying migrations to database...");
    context.Database.Migrate();
    Console.WriteLine("Database ready!\n");

    if (!context.Departments.Any())
    {
        Console.WriteLine("--- Seeding Initial Data ---");
        
        var itDept = new Department
        {
            Name = "IT Department",
            Location = "Building A"
        };

        var hrDept = new Department
        {
            Name = "HR Department",
            Location = "Building B"
        };

        var salesDept = new Department
        {
            Name = "Sales Department",
            Location = "Building C"
        };

        context.Departments.AddRange(itDept, hrDept, salesDept);
        context.SaveChanges();
        Console.WriteLine("✓ Departments created");

        var emp1 = new Employee
        {
            Name = "John Doe",
            Email = "john.doe@company.com",
            Salary = 75000,
            HireDate = DateTime.Now.AddYears(-2),
            DepartmentId = itDept.DepartmentId
        };

        var emp2 = new Employee
        {
            Name = "Jane Smith",
            Email = "jane.smith@company.com",
            Salary = 65000,
            HireDate = DateTime.Now.AddYears(-1),
            DepartmentId = itDept.DepartmentId
        };

        var emp3 = new Employee
        {
            Name = "Bob Johnson",
            Email = "bob.johnson@company.com",
            Salary = 55000,
            HireDate = DateTime.Now.AddMonths(-6),
            DepartmentId = hrDept.DepartmentId
        };

        var emp4 = new Employee
        {
            Name = "Alice Williams",
            Email = "alice.williams@company.com",
            Salary = 70000,
            HireDate = DateTime.Now.AddMonths(-3),
            DepartmentId = salesDept.DepartmentId
        };

        var emp5 = new Employee
        {
            Name = "Charlie Brown",
            Email = "charlie.brown@company.com",
            Salary = 85000,
            HireDate = DateTime.Now.AddYears(-3),
            DepartmentId = itDept.DepartmentId
        };

        context.Employees.AddRange(emp1, emp2, emp3, emp4, emp5);
        context.SaveChanges();
        Console.WriteLine("✓ Employees created");

        var project1 = new Project
        {
            ProjectName = "Website Redesign",
            Description = "Redesign company website",
            StartDate = DateTime.Now.AddMonths(-3)
        };

        var project2 = new Project
        {
            ProjectName = "Mobile App Development",
            Description = "Develop mobile application",
            StartDate = DateTime.Now.AddMonths(-2)
        };

        var project3 = new Project
        {
            ProjectName = "HR System Upgrade",
            Description = "Upgrade HR management system",
            StartDate = DateTime.Now.AddMonths(-1)
        };

        var project4 = new Project
        {
            ProjectName = "Cloud Migration",
            Description = "Migrate infrastructure to cloud",
            StartDate = DateTime.Now.AddMonths(-4)
        };

        context.Projects.AddRange(project1, project2, project3, project4);
        context.SaveChanges();
        Console.WriteLine("✓ Projects created");

        var empProj1 = new EmployeeProject
        {
            EmployeeId = emp1.EmployeeId,
            ProjectId = project1.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-3),
            Role = "Lead Developer"
        };

        var empProj2 = new EmployeeProject
        {
            EmployeeId = emp1.EmployeeId,
            ProjectId = project2.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-2),
            Role = "Backend Developer"
        };

        var empProj3 = new EmployeeProject
        {
            EmployeeId = emp2.EmployeeId,
            ProjectId = project1.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-3),
            Role = "Frontend Developer"
        };

        var empProj4 = new EmployeeProject
        {
            EmployeeId = emp3.EmployeeId,
            ProjectId = project3.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-1),
            Role = "Project Manager"
        };

        var empProj5 = new EmployeeProject
        {
            EmployeeId = emp5.EmployeeId,
            ProjectId = project4.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-4),
            Role = "Solutions Architect"
        };

        var empProj6 = new EmployeeProject
        {
            EmployeeId = emp5.EmployeeId,
            ProjectId = project1.ProjectId,
            AssignedDate = DateTime.Now.AddMonths(-3),
            Role = "Technical Lead"
        };

        context.EmployeeProjects.AddRange(empProj1, empProj2, empProj3, empProj4, empProj5, empProj6);
        context.SaveChanges();
        Console.WriteLine("✓ Employee-Project assignments created\n");
    }

    var employeeRepo = new EmployeeRepository(context);

    Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║         REPOSITORY PATTERN DEMONSTRATION                    ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

    Console.WriteLine("--- 1. Basic Repository Operations ---");
    var allEmployees = await employeeRepo.GetAllAsync();
    Console.WriteLine($"Total Employees (using Repository): {allEmployees.Count()}");

    var employeeById = await employeeRepo.GetByIdAsync(1);
    if (employeeById != null)
    {
        Console.WriteLine($"Employee by ID 1: {employeeById.Name} - {employeeById.Email}");
    }

    var highSalaryEmployees = await employeeRepo.FindAsync(e => e.Salary > 70000);
    Console.WriteLine($"Employees with salary > $70,000: {highSalaryEmployees.Count()}");

    Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║         ADVANCED LINQ QUERIES DEMONSTRATION                  ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

    Console.WriteLine("--- 2. Employees with Departments (Include & OrderBy) ---");
    var employeesWithDepts = await employeeRepo.GetEmployeesWithDepartmentsAsync();
    foreach (var emp in employeesWithDepts)
    {
        Console.WriteLine($"  {emp.Name} ({emp.Email}) - {emp.Department.Name} - ${emp.Salary:N2}");
    }

    Console.WriteLine("\n--- 3. Employees by Salary Range (Where & OrderByDescending) ---");
    var salaryRangeEmployees = await employeeRepo.GetEmployeesBySalaryRangeAsync(60000, 80000);
    foreach (var emp in salaryRangeEmployees)
    {
        Console.WriteLine($"  {emp.Name}: ${emp.Salary:N2} - {emp.Department.Name}");
    }

    Console.WriteLine("\n--- 4. Employees Grouped by Department (GroupBy & Select) ---");
    var groupedEmployees = await employeeRepo.GetEmployeesGroupedByDepartmentAsync();
    foreach (dynamic group in groupedEmployees)
    {
        Console.WriteLine($"\n  Department: {group.DepartmentName}");
        Console.WriteLine($"  Employee Count: {group.EmployeeCount}");
        Console.WriteLine("  Employees:");
        foreach (var emp in group.Employees)
        {
            Console.WriteLine($"    - {emp.Name} (${emp.Salary:N2})");
        }
    }

    Console.WriteLine("\n--- 5. Salary Statistics by Department (GroupBy with Aggregations) ---");
    var salaryStats = await employeeRepo.GetEmployeeSalaryStatisticsByDepartmentAsync();
    foreach (dynamic stat in salaryStats)
    {
        Console.WriteLine($"\n  Department: {stat.DepartmentName}");
        Console.WriteLine($"    Employee Count: {stat.EmployeeCount}");
        Console.WriteLine($"    Average Salary: ${stat.AverageSalary:N2}");
        Console.WriteLine($"    Min Salary: ${stat.MinSalary:N2}");
        Console.WriteLine($"    Max Salary: ${stat.MaxSalary:N2}");
        Console.WriteLine($"    Total Salary Expense: ${stat.TotalSalary:N2}");
    }

    Console.WriteLine("\n--- 6. Employees with Project Count (Nested Select & Count) ---");
    var employeesWithProjects = await employeeRepo.GetEmployeesWithProjectCountAsync();
    foreach (dynamic emp in employeesWithProjects)
    {
        Console.WriteLine($"\n  {emp.Name} ({emp.DepartmentName})");
        Console.WriteLine($"    Total Projects: {emp.ProjectCount}");
        if (emp.ProjectCount > 0)
        {
            Console.WriteLine("    Projects:");
            foreach (var proj in emp.Projects)
            {
                Console.WriteLine($"      - {proj.ProjectName} (Role: {proj.Role})");
            }
        }
    }

    Console.WriteLine("\n--- 7. Top Earners by Department (GroupBy & FirstOrDefault) ---");
    var topEarners = await employeeRepo.GetTopEarnersByDepartmentAsync();
    foreach (dynamic emp in topEarners)
    {
        Console.WriteLine($"  {emp.DepartmentName}: {emp.Name} - ${emp.Salary:N2}");
    }

    Console.WriteLine("\n--- 8. Employee-Project Details (LINQ Join Query) ---");
    var employeeProjectDetails = await employeeRepo.GetEmployeeProjectDetailsAsync();
    foreach (dynamic detail in employeeProjectDetails)
    {
        Console.WriteLine($"  {detail.EmployeeName} ({detail.DepartmentName})");
        Console.WriteLine($"    Project: {detail.ProjectName}");
        Console.WriteLine($"    Role: {detail.Role}");
        Console.WriteLine($"    Assigned: {detail.AssignedDate:yyyy-MM-dd}");
        Console.WriteLine();
    }

    Console.WriteLine("\n--- 9. Department Employee Count (Left Join with GroupJoin) ---");
    var deptEmployeeCount = await employeeRepo.GetDepartmentEmployeeCountAsync();
    foreach (dynamic dept in deptEmployeeCount)
    {
        Console.WriteLine($"  {dept.DepartmentName} ({dept.Location})");
        Console.WriteLine($"    Employees: {dept.EmployeeCount}");
        Console.WriteLine($"    Total Salary Expense: ${dept.TotalSalaryExpense:N2}");
        Console.WriteLine($"    Average Salary: ${dept.AverageSalary:N2}");
        Console.WriteLine();
    }

    Console.WriteLine("\n--- 10. Employees Hired in Last 12 Months (Where with DateTime) ---");
    var recentHires = await employeeRepo.GetEmployeesHiredInLastMonthsAsync(12);
    foreach (var emp in recentHires)
    {
        Console.WriteLine($"  {emp.Name} - Hired: {emp.HireDate:yyyy-MM-dd} ({emp.Department.Name})");
    }

    Console.WriteLine("\n--- 11. Complex Join Query (Multiple Joins & Grouping) ---");
    var complexQuery = await employeeRepo.GetComplexJoinQueryAsync();
    foreach (dynamic result in complexQuery)
    {
        Console.WriteLine($"\n  Employee: {result.Name}");
        Console.WriteLine($"  Email: {result.Email}");
        Console.WriteLine($"  Salary: ${result.Salary:N2}");
        Console.WriteLine($"  Department: {result.DepartmentName} ({result.DepartmentLocation})");
        Console.WriteLine($"  Hire Date: {result.HireDate:yyyy-MM-dd}");
        Console.WriteLine($"  Total Projects: {result.ProjectCount}");
        if (result.ProjectCount > 0)
        {
            Console.WriteLine("  Projects:");
            foreach (var proj in result.Projects)
            {
                Console.WriteLine($"    - {proj.ProjectName} (Role: {proj.Role}, Assigned: {proj.AssignedDate:yyyy-MM-dd})");
            }
        }
    }

    Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║         REPOSITORY CRUD OPERATIONS                           ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

    Console.WriteLine("--- 12. CREATE: Adding new employee via Repository ---");
    var newEmployee = new Employee
    {
        Name = "David Miller",
        Email = "david.miller@company.com",
        Salary = 72000,
        HireDate = DateTime.Now,
        DepartmentId = context.Departments.First(d => d.Name == "IT Department").DepartmentId
    };
    await employeeRepo.AddAsync(newEmployee);
    await employeeRepo.SaveChangesAsync();
    Console.WriteLine($"  ✓ Added: {newEmployee.Name} (ID: {newEmployee.EmployeeId})");

    Console.WriteLine("\n--- 13. UPDATE: Updating employee via Repository ---");
    var empToUpdate = (await employeeRepo.FindAsync(e => e.Name == "John Doe")).FirstOrDefault();
    if (empToUpdate != null)
    {
        var oldSalary = empToUpdate.Salary;
        empToUpdate.Salary = 82000;
        employeeRepo.Update(empToUpdate);
        await employeeRepo.SaveChangesAsync();
        Console.WriteLine($"  ✓ Updated {empToUpdate.Name}: ${oldSalary:N2} → ${empToUpdate.Salary:N2}");
    }

    Console.WriteLine("\n\n--- Database Statistics ---");
    Console.WriteLine($"Total Departments: {context.Departments.Count()}");
    Console.WriteLine($"Total Employees: {context.Employees.Count()}");
    Console.WriteLine($"Total Projects: {context.Projects.Count()}");
    Console.WriteLine($"Total Assignments: {context.EmployeeProjects.Count()}");
}

Console.WriteLine("\n\n=== Application Completed Successfully ===");
