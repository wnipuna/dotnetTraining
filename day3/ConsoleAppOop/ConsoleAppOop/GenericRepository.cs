public class GenericRepository<T>
{
    private List<T> Items = new List<T>();

    public void Add(T item)
    {
        Items.Add(item);
    }

    public List<T> GetAll()
    {
        return Items;
    }
}