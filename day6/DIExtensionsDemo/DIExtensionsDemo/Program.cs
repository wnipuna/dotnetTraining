using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DIExtensionsDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection();

            // Register DI services
            services.AddSingleton<ISingletonService, LifetimeService>();
            services.AddScoped<IScopedService, LifetimeService>();
            services.AddTransient<ITransientService, LifetimeService>();

            services.AddScoped<IEmployeeService, EmployeeService>();

            using var provider = services.BuildServiceProvider();

            // Observe lifetimes across two scopes
            PrintLifetimeIds(provider, scopeName: "Scope-1");
            PrintLifetimeIds(provider, scopeName: "Scope-2");

            // Resolve IEmployeeService and use extension methods
            using (var scope = provider.CreateScope())
            {
                var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
                var employees = await employeeService.GetAllAsync();

                // Format names using extension method
                var formatted = employees
                    .Select(e => e with { Name = e.Name.FormatName() })
                    .ToList();

                Console.WriteLine("== All Employees (Formatted Names) ==");
                foreach (var e in formatted)
                    Console.WriteLine($"{e.Id,2} | {e.Name,-12} | {e.Age,2} | {e.Department,-8}");

                // Filter by department
                var it = formatted.FilterByDepartment("IT").ToList();
                Console.WriteLine("\n== IT Department ==");
                foreach (var e in it)
                    Console.WriteLine($"{e.Id,2} | {e.Name,-12} | {e.Age,2} | {e.Department,-8}");

                // Average age
                var avgAge = formatted.AverageAge();
                Console.WriteLine($"\nAverage Age: {avgAge:F1}");
            }
        }

        static void PrintLifetimeIds(ServiceProvider provider, string scopeName)
        {
            using var scope = provider.CreateScope();
            var sp = scope.ServiceProvider;

            var s1 = sp.GetRequiredService<ISingletonService>();
            var s2 = sp.GetRequiredService<ISingletonService>();

            var sc1 = sp.GetRequiredService<IScopedService>();
            var sc2 = sp.GetRequiredService<IScopedService>();

            var t1 = sp.GetRequiredService<ITransientService>();
            var t2 = sp.GetRequiredService<ITransientService>();

            Console.WriteLine($"\n== {scopeName} Lifetimes ==");
            Console.WriteLine($"Singleton: {s1.Id} == {s2.Id}");
            Console.WriteLine($"Scoped:    {sc1.Id} == {sc2.Id}");
            Console.WriteLine($"Transient: {t1.Id} != {t2.Id}");
        }
    }
}
