using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DIExtensionsDemo
{
    public static class EmployeeExtensions
    {
        public static string FormatName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name ?? string.Empty;
            name = name.Trim().ToLowerInvariant();
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }

        public static IEnumerable<Employee> FilterByDepartment(this IEnumerable<Employee> source, string department)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(department)) return source;
            return source.Where(e => string.Equals(e.Department, department, StringComparison.OrdinalIgnoreCase));
        }

        public static double AverageAge(this IEnumerable<Employee> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            var list = source as IList<Employee> ?? source.ToList();
            if (list.Count == 0) return 0d;
            return list.Average(e => e.Age);
        }
    }
}
