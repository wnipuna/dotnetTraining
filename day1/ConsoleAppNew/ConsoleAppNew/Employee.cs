class Employee
{
    public int Id { get; set; }
    public String? Name { get; set; }
    public int Age { get; set; }
    public String? Department { get; set; }

    public Employee(int id, string? name, int age, string? department)
    {
        this.Id = id;
        this.Name = name;
        this.Age = age;
        this.Department = department;
    }

    public Employee() { }

    public override string ToString()
    {
        return $"ID: {Id},Name: {Name}, Age: {Age}, Department: {Department}";
    }
}
