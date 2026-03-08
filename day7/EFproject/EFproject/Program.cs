using EFproject;

class Program
{
    static async Task Main(string[] args)
    {

        var employees = await GetEmployeesAsync();

        Console.WriteLine("--- 1. LINQ Query Syntax vs Method Syntax ---\n");
        DemonstrateQuerySyntaxVsMethodSyntax(employees);

        Console.WriteLine("\n--- 2. Filter Employees by Age (Age > 30) ---\n");
        FilterEmployeesByAge(employees, 30);

        Console.WriteLine("\n--- 3. Filter Employees by Department ---\n");
        await FilterEmployeesByDepartmentAsync(employees, "IT");

        Console.WriteLine("\n--- 4. LINQ Projections (Display Specific Properties) ---\n");
        DisplayEmployeeProjections(employees);

        Console.WriteLine("\n--- 5. LINQ Aggregate Functions ---\n");
        DisplayAggregateStatistics(employees);

        Console.WriteLine("\n--- 6. Advanced LINQ Operations ---\n");
        DemonstrateAdvancedLinq(employees);
    }

    static async Task<List<Employee>> GetEmployeesAsync()
    {

        var departments = new List<Department>
        {
            new Department { Id = 1, Name = "IT" },
            new Department { Id = 2, Name = "HR" },
            new Department { Id = 3, Name = "Finance" },
            new Department { Id = 4, Name = "Marketing" }
        };

        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", Age = 28, DepartmentId = 1, Department = departments[0] },
            new Employee { Id = 2, Name = "Jane Smith", Age = 35, DepartmentId = 1, Department = departments[0] },
            new Employee { Id = 3, Name = "Bob Johnson", Age = 42, DepartmentId = 2, Department = departments[1] },
            new Employee { Id = 4, Name = "Alice Williams", Age = 31, DepartmentId = 2, Department = departments[1] },
            new Employee { Id = 5, Name = "Charlie Brown", Age = 29, DepartmentId = 3, Department = departments[2] },
            new Employee { Id = 6, Name = "Diana Prince", Age = 38, DepartmentId = 3, Department = departments[2] },
            new Employee { Id = 7, Name = "Eve Davis", Age = 45, DepartmentId = 1, Department = departments[0] },
            new Employee { Id = 8, Name = "Frank Miller", Age = 33, DepartmentId = 4, Department = departments[3] },
            new Employee { Id = 9, Name = "Grace Lee", Age = 27, DepartmentId = 4, Department = departments[3] },
            new Employee { Id = 10, Name = "Henry Wilson", Age = 40, DepartmentId = 2, Department = departments[1] }
        };
        return employees;
    }

    static void DemonstrateQuerySyntaxVsMethodSyntax(List<Employee> employees)
    {
        Console.WriteLine("Query Syntax (SQL-like):");
        var queryResult = from emp in employees
                          where emp.Age > 30
                          orderby emp.Age descending
                          select emp;

        foreach (var emp in queryResult)
        {
            Console.WriteLine($"  {emp.Name}, Age: {emp.Age}");
        }

        Console.WriteLine("\nMethod Syntax (Extension Methods):");
        var methodResult = employees
            .Where(emp => emp.Age > 30)
            .OrderByDescending(emp => emp.Age);

        foreach (var emp in methodResult)
        {
            Console.WriteLine($"  {emp.Name}, Age: {emp.Age}");
        }
    }

    static void FilterEmployeesByAge(List<Employee> employees, int minAge)
    {
        var filteredEmployees = employees.Where(e => e.Age > minAge).ToList();

        Console.WriteLine($"Employees older than {minAge}:");
        foreach (var emp in filteredEmployees)
        {
            Console.WriteLine($"  {emp.Name} - Age: {emp.Age}, Department: {emp.Department.Name}");
        }
        Console.WriteLine($"Total: {filteredEmployees.Count} employees");
    }

    static async Task FilterEmployeesByDepartmentAsync(List<Employee> employees, string departmentName)
    {
        Console.WriteLine($"Simulating async filter for department: {departmentName}...");
        await Task.Delay(500);

        var filteredEmployees = employees
            .Where(e => e.Department.Name == departmentName)
            .OrderBy(e => e.Name)
            .ToList();

        Console.WriteLine($"Employees in {departmentName} department:");
        foreach (var emp in filteredEmployees)
        {
            Console.WriteLine($"  {emp.Name} - Age: {emp.Age}");
        }
        Console.WriteLine($"Total: {filteredEmployees.Count} employees");
    }

    static void DisplayEmployeeProjections(List<Employee> employees)
    {
        Console.WriteLine("1: Anonymous Type (Name and Department only)");
        var projection1 = employees.Select(e => new { e.Name, Department = e.Department.Name });
        foreach (var item in projection1)
        {
            Console.WriteLine($"  {item.Name} works in {item.Department}");
        }

        Console.WriteLine("\n2: Custom Format String");
        var projection2 = employees.Select(e => $"{e.Name} ({e.Age} years old) - {e.Department.Name}");
        foreach (var item in projection2)
        {
            Console.WriteLine($"  {item}");
        }

        Console.WriteLine("\n3: Specific Properties with Calculation");
        var projection3 = employees.Select(e => new
        {
            EmployeeName = e.Name,
            AgeGroup = e.Age < 30 ? "Young" : e.Age < 40 ? "Mid-Career" : "Senior",
            e.Department.Name
        });
        foreach (var item in projection3)
        {
            Console.WriteLine($"  {item.EmployeeName} - {item.AgeGroup} ({item.Name})");
        }
    }

    static void DisplayAggregateStatistics(List<Employee> employees)
    {
        Console.WriteLine("Aggregate Functions:");
        Console.WriteLine($"  Total Employees: {employees.Count()}");
        Console.WriteLine($"  Average Age: {employees.Average(e => e.Age):F2} years");
        Console.WriteLine($"  Maximum Age: {employees.Max(e => e.Age)} years");
        Console.WriteLine($"  Minimum Age: {employees.Min(e => e.Age)} years");
        Console.WriteLine($"  Sum of All Ages: {employees.Sum(e => e.Age)} years");

        Console.WriteLine("\nAggregate by Department:");
        var departmentStats = employees
            .GroupBy(e => e.Department.Name)
            .Select(g => new
            {
                Department = g.Key,
                Count = g.Count(),
                AvgAge = g.Average(e => e.Age),
                MaxAge = g.Max(e => e.Age)
            });

        foreach (var stat in departmentStats)
        {
            Console.WriteLine($"  {stat.Department}: {stat.Count} employees, Avg Age: {stat.AvgAge:F2}, Max Age: {stat.MaxAge}");
        }
    }

    static void DemonstrateAdvancedLinq(List<Employee> employees)
    {
        Console.WriteLine("GroupBy Department:");
        var groupedByDept = employees.GroupBy(e => e.Department.Name);
        foreach (var group in groupedByDept)
        {
            Console.WriteLine($"  {group.Key}: {string.Join(", ", group.Select(e => e.Name))}");
        }

        Console.WriteLine("\nOrderBy Multiple Criteria (Department, then Age):");
        var orderedEmployees = employees
            .OrderBy(e => e.Department.Name)
            .ThenByDescending(e => e.Age);
        foreach (var emp in orderedEmployees)
        {
            Console.WriteLine($"  {emp.Department.Name} - {emp.Name} (Age: {emp.Age})");
        }

        Console.WriteLine("\nFirst, Last, and Single Operations:");
        var firstEmployee = employees.First();
        Console.WriteLine($"  First Employee: {firstEmployee.Name}");

        var lastEmployee = employees.Last();
        Console.WriteLine($"  Last Employee: {lastEmployee.Name}");

        var oldestEmployee = employees.OrderByDescending(e => e.Age).First();
        Console.WriteLine($"  Oldest Employee: {oldestEmployee.Name} (Age: {oldestEmployee.Age})");

        Console.WriteLine("\nAny and All Operations:");
        Console.WriteLine($"  Any employee over 40? {employees.Any(e => e.Age > 40)}");
        Console.WriteLine($"  All employees over 18? {employees.All(e => e.Age > 18)}");
    }
}
