namespace MiniPGN.Utils;

public static class GeneralUtils
{
    public static ulong CharLenght(this IEnumerable<string> strList)
    {
        return strList.Sum(s => (ulong)s.Length);
    }

    public static ulong Sum(this IEnumerable<ulong> list)
    {
        return list.Aggregate((a, b) => a + b);
    }
    
    public static ulong Sum<T>(this IEnumerable<T> list, Func<T, ulong> func)
    {
        return list.Select(func).Sum();
    }
}