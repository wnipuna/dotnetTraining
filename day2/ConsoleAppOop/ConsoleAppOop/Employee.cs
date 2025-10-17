public class Employee
{
    private int Id;
    private string Name;
    private string Department;

    public int GetId
    {
        get => Id;
        set => Id = value;
    }

    public string GetName
    {
        get => Name;
        set => Name = value;
    }

    public string GetDepartment
    {
        get => Department;
        set => Department = value;
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"ID: {Id}, Name: {Name}, Department: {Department}");
    }
}