IEmployeeService employeeService = new EmployeeService();

Employee Employee = new Employee { GetId = 1, GetName = "Nipuna", GetDepartment = "SE" };
Manager manager = new Manager { GetId = 2, GetName = "Ruwantha", GetDepartment = "IT", TeamSize = 5 };

employeeService.AddEmployee(Employee);
employeeService.AddEmployee(manager);

Console.WriteLine("All Employees:");
foreach (var emp in employeeService.GetAllEmployees())
{
    emp.DisplayDetails();
}

Console.WriteLine("\nGet Employee by ID (2):");
var found = employeeService.GetEmployeeById(2);
found?.DisplayDetails();

GenericRepository<Employee> EmployeeRepo = new GenericRepository<Employee>();
EmployeeRepo.Add(Employee);
EmployeeRepo.Add(manager);

Console.WriteLine("\nGeneric Repository contents:");
foreach (var e in EmployeeRepo.GetAll())
{
    e.DisplayDetails();
}