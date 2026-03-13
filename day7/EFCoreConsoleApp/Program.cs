using EFCoreConsoleApp.Data;
using EFCoreConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== EF Core Console Application ===\n");

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

        context.Employees.AddRange(emp1, emp2, emp3);
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

        context.Projects.AddRange(project1, project2, project3);
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

        context.EmployeeProjects.AddRange(empProj1, empProj2, empProj3, empProj4);
        context.SaveChanges();
        Console.WriteLine("✓ Employee-Project assignments created\n");
    }

    Console.WriteLine("--- Demonstrating One-to-Many Relationship (Department → Employees) ---");
    var departments = context.Departments
        .Include(d => d.Employees)
        .ToList();

    foreach (var dept in departments)
    {
        Console.WriteLine($"\n{dept.Name} ({dept.Location}):");
        Console.WriteLine($"  Employees: {dept.Employees.Count}");
        foreach (var emp in dept.Employees)
        {
            Console.WriteLine($"    - {emp.Name} ({emp.Email}) - Salary: ${emp.Salary:N2}");
        }
    }

    Console.WriteLine("\n\n--- Demonstrating Many-to-Many Relationship (Employees ↔ Projects) ---");
    var employees = context.Employees
        .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
        .ToList();

    foreach (var emp in employees)
    {
        Console.WriteLine($"\n{emp.Name}:");
        Console.WriteLine($"  Assigned Projects: {emp.EmployeeProjects.Count}");
        foreach (var ep in emp.EmployeeProjects)
        {
            Console.WriteLine($"    - {ep.Project.ProjectName} (Role: {ep.Role})");
        }
    }

    Console.WriteLine("\n\n--- Demonstrating CRUD Operations ---");
    
    Console.WriteLine("\n1. CREATE: Adding new employee");
    var newEmployee = new Employee
    {
        Name = "Alice Williams",
        Email = "alice.williams@company.com",
        Salary = 70000,
        HireDate = DateTime.Now,
        DepartmentId = departments.First(d => d.Name == "Sales Department").DepartmentId
    };
    context.Employees.Add(newEmployee);
    context.SaveChanges();
    Console.WriteLine($"   ✓ Added: {newEmployee.Name} (ID: {newEmployee.EmployeeId})");

    Console.WriteLine("\n2. READ: Querying employees with salary > $60,000");
    var highEarners = context.Employees
        .Where(e => e.Salary > 60000)
        .Include(e => e.Department)
        .OrderByDescending(e => e.Salary)
        .ToList();
    
    foreach (var emp in highEarners)
    {
        Console.WriteLine($"   - {emp.Name}: ${emp.Salary:N2} ({emp.Department.Name})");
    }

    Console.WriteLine("\n3. UPDATE: Updating employee salary");
    var empToUpdate = context.Employees.First(e => e.Name == "John Doe");
    var oldSalary = empToUpdate.Salary;
    empToUpdate.Salary = 80000;
    context.SaveChanges();
    Console.WriteLine($"   ✓ Updated {empToUpdate.Name}: ${oldSalary:N2} → ${empToUpdate.Salary:N2}");

    Console.WriteLine("\n4. DELETE: Removing an employee-project assignment");
    var assignmentToDelete = context.EmployeeProjects.FirstOrDefault();
    if (assignmentToDelete != null)
    {
        var empName = context.Employees.Find(assignmentToDelete.EmployeeId)?.Name;
        var projName = context.Projects.Find(assignmentToDelete.ProjectId)?.ProjectName;
        context.EmployeeProjects.Remove(assignmentToDelete);
        context.SaveChanges();
        Console.WriteLine($"   ✓ Removed assignment: {empName} from {projName}");
    }

    Console.WriteLine("\n\n--- Database Statistics ---");
    Console.WriteLine($"Total Departments: {context.Departments.Count()}");
    Console.WriteLine($"Total Employees: {context.Employees.Count()}");
    Console.WriteLine($"Total Projects: {context.Projects.Count()}");
    Console.WriteLine($"Total Assignments: {context.EmployeeProjects.Count()}");
}

Console.WriteLine("\n\n=== Application Completed Successfully ===");
