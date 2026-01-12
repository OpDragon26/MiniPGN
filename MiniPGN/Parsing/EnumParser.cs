namespace MiniPGN.Parsing;

public static class EnumParser
{
    public static IEnumerable<T> Extract<T>(this IEnumerator<T> enumerator, int count)
    {
        for (int i = 0; i < count; i++)
            yield return enumerator.Next();
    }

    public static T Next<T>(this IEnumerator<T> enumerator)
    {
        T current = enumerator.Current;
        enumerator.MoveNext();
        return current;
    }
}