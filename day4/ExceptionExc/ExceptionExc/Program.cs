using System;
using Serilog;
using ExceptionExc;

namespace ExceptionExc
{
    public class ExceptionTest
    {
        static double SafeDivision(double x, double y)
        {
            if (y == 0)
                throw new DivideByZeroException();
            return x / y;
        }

        public static void Main()
        {
            // Input for test purposes. Change the values to see
            // exception handling behavior.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            GlobalExceptionHandler.Register();

            try
            {
                Console.Write("Enter first number: ");
                if (!double.TryParse(Console.ReadLine(), out var a))
                    throw new ArgumentException("Invalid input for first number");

                Console.Write("Enter second number: ");
                if (!double.TryParse(Console.ReadLine(), out var b))
                    throw new ArgumentException("Invalid input for second number");

                var result = SafeDivision(a, b);
                Console.WriteLine($"{a} divided by {b} = {result}");
                Log.Information("Division successful: {A} / {B} = {Result}", a, b, result);
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Attempted divide by zero.");
                Log.Error(ex, "Divide by zero detected");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Log.Warning(ex, "Invalid user input");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.");
                Log.Error(ex, "Unexpected error during division flow");
            }

            try
            {
                Console.Write("Enter employee id to lookup: ");
                if (!int.TryParse(Console.ReadLine(), out var empId))
                    throw new ArgumentException("Invalid input for employee id");

                var name = GetEmployeeName(empId);
                Console.WriteLine($"Employee: {name}");
                Log.Information("Employee lookup succeeded for {EmpId}", empId);
            }
            catch (EmployeeNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex, "Employee not found");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Log.Warning(ex, "Invalid employee id input");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.");
                Log.Error(ex, "Unexpected error during employee lookup");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static string GetEmployeeName(int id)
        {
            if (id == 1)
                return "Alice";
            throw new EmployeeNotFoundException($"Employee with id {id} not found");
        }
    }
}