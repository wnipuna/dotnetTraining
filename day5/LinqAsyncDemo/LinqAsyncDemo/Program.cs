using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqAsyncDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var data = new DataService();

            // 1) Async/await simulating DB call
            var employees = await data.GetEmployeesAsync();

            // 2) LINQ filters
            var adults = employees.Where(e => e.Age >= 18).ToList();
            var itEmployees = employees.Where(e => e.Department.Equals("IT", StringComparison.OrdinalIgnoreCase)).ToList();
            var seniorEmployees = employees.Where(e => e.Age >= 40 && e.Salary >= 80000).ToList();

            // 3) LINQ projections (only specific properties)
            var nameAndDept = employees.Select(e => new { e.Name, e.Department }).ToList();
            var summaries = employees.Select(e => new EmployeeSummary(e.Id, e.Name, e.Department)).ToList();

            // 4) LINQ aggregate functions
            var avgAge = employees.Average(e => e.Age);
            var maxSalary = employees.Max(e => e.Salary);
            var itCount = employees.Count(e => e.Department == "IT");

            // Output demo
            Console.WriteLine("== All Employees ==");
            Print(employees);

            Console.WriteLine("\n== Adults (Age >= 18) ==");
            Print(adults);

            Console.WriteLine("\n== IT Department ==");
            Print(itEmployees);

            Console.WriteLine("\n== Senior (Age >= 40 & Salary >= 80000) ==");
            Print(seniorEmployees);

            Console.WriteLine("\n== Projection: Name & Department ==");
            foreach (var nd in nameAndDept)
                Console.WriteLine($"{nd.Name} - {nd.Department}");

            Console.WriteLine("\n== Projection: Summaries ==");
            foreach (var s in summaries)
                Console.WriteLine($"{s.Id}: {s.Name} ({s.Department})");

            Console.WriteLine("\n== Aggregates ==");
            Console.WriteLine($"Average Age: {avgAge:F1}");
            Console.WriteLine($"Max Salary: {maxSalary:C}");
            Console.WriteLine($"IT Count: {itCount}");

            // Example of additional async filtering (simulated sequential async calls)
            var highEarners = await data.FilterAsync(employees, e => e.Salary > 90000);
            Console.WriteLine("\n== High Earners (async filter) ==");
            Print(highEarners);
        }

        static void Print(IEnumerable<Employee> employees)
        {
            foreach (var e in employees)
                Console.WriteLine($"{e.Id,2} | {e.Name,-12} | {e.Age,2} | {e.Department,-10} | {e.Salary,10:C}");
        }
    }

    public record EmployeeSummary(int Id, string Name, string Department);

    internal class DataService
    {
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            await Task.Delay(200); // simulate I/O latency
            return new List<Employee>
            {
                new(1, "Alice", 30, "IT", 95000),
                new(2, "Bob", 22, "HR", 52000),
                new(3, "Charlie", 45, "Finance", 120000),
                new(4, "Diana", 28, "IT", 85000),
                new(5, "Evan", 19, "Sales", 48000),
                new(6, "Fiona", 41, "IT", 102000),
                new(7, "George", 38, "Marketing", 78000),
                new(8, "Hannah", 26, "Finance", 70000)
            };
        }

        public async Task<List<Employee>> FilterAsync(IEnumerable<Employee> source, Func<Employee, bool> predicate)
        {
            await Task.Delay(100); // simulate additional work
            return source.Where(predicate).ToList();
        }
    }
}
