// See https://aka.ms/new-console-template for more information
Console.Write("How many Employees?: ");
int Employees = int.Parse(Console.ReadLine());

List<Employee> EmployeesList = new List<Employee>();

for (int i = 1; i <= Employees; i++)
{
    Console.WriteLine("Enter details of Employee " +  i);
    Employee EmpL = new();
    EmpL.Id = i;
    Console.Write("Enter name : ");
    EmpL.Name = Console.ReadLine();

    String Name = EmpL.Name;
    EmpL.Name = Char.ToUpperInvariant(Name[0]) + Name.Substring(1);

    int Age;
    while (true)
    {
        Console.Write("Enter age: ");
        string Input = Console.ReadLine();

        if (!int.TryParse(Input, out Age))
        {
            Console.WriteLine("Please enter a valid number for age.");
            continue;
        }
        if (Age < 0)
        {
            Console.WriteLine("Age cannot be negative. Please try again.");
            continue;
        }

        break;
    }

    EmpL.Age = Age;

    Console.Write("Enter department : ");
    EmpL.Department = Console.ReadLine();

    EmployeesList.Add(EmpL);
}

Console.WriteLine("\nAll Employees from list");
for (int i = 0; i < Employees; i++)
{
    Console.WriteLine(EmployeesList[i]);
}

Dictionary<int, Employee> EmpDictionary = EmployeesList.ToDictionary(x => x.Id);

Console.WriteLine("\nAll Employees from dictionary");

foreach (var emplo in EmpDictionary)
{
    Console.WriteLine(emplo);
}