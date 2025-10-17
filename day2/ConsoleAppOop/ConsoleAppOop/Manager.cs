public class Manager : Employee
{
    public int TeamSize { get; set; }

    public override void DisplayDetails()
    {
        Console.WriteLine($"[Manager] ID: {GetId}, Name: {GetName}, Department: {GetDepartment}, Team Size: {TeamSize}");
    }
}