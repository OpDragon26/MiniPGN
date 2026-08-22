namespace MiniPGN.Utils;

public class Counter<T> : Dictionary<T, int>
{
    public void AddCount(T item)
    {
        if (TryGetValue(item, out int count))
            this[item] = count + 1;
        else
            Add(item, 1);
    }

    public IEnumerable<KeyValuePair<T, int>> GetSorted()
    {
        return this.OrderBy(x => x.Value);
    }
}