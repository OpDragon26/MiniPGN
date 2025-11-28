namespace MiniPGN.Chess.Bitboards;

public static class MagicNumberGenerator
{
    public static (ulong number, int shift, ulong highest) GenerateNew(ulong[] combinations, int shift = 48, bool improveShift = true)
    {
        ulong number;
        ulong[] result;
        int extraShift = 0;
        while (true)
        {
            number = Chess.Utils.RandomUlong();
            result = combinations.MagicAll(number, shift);

            if (result.IsDistinct())
            {
                if (!improveShift)
                    break;

                for (extraShift = 1; extraShift < 63 - shift; extraShift++)
                {
                    ulong[] improved = combinations.MagicAll(number, shift + extraShift);

                    if (improved.IsDistinct())
                        result = improved;
                    else
                        break;
                }

                extraShift--;

                break;
            }
        }

        return (number, shift + extraShift, result.Max() + 1);
    }

    public static (ulong number, int shift, ulong highest) GenerateMulti(ulong[] combinations, int shift = 48,
        int threads = 5)
    {
        bool found = false;
        (ulong number, int shift, ulong highest) magic = (ulong.MaxValue, -1, int.MaxValue);

        for (int t = 0; t < threads; t++)
        {
            new Thread(() =>
            {
                while (!found)
                {
                    ulong number = Chess.Utils.RandomUlong();
                    ulong[] result = combinations.MagicAll(number, shift);

                    if (result.IsDistinct())
                    {
                        found = true;
                        magic = (number, shift, result.Max() + 1);
                    }
                }
            }).Start();
        }

        while (true)
        {
            if (found) break;
            Thread.Sleep(100);
        }

        return magic;
    }

    public static (ulong number, int shift, ulong highest) GenerateBestMagic(ulong[] combinations, int iterations, int shift = 48, bool improveShift = true)
    {
        (ulong number, int shift, ulong highest) best = (ulong.MaxValue, -1, int.MaxValue);

        for (int e = 0; e < iterations; e++)
        {
            (ulong number, int shift, ulong highest) newNumber = GenerateNew(combinations, shift, improveShift);

            if (newNumber.shift > best.shift)
                best = newNumber;
            else if (newNumber.shift == best.shift && newNumber.highest < best.highest)
                best = newNumber;
        }

        return best;
    }

    private static ulong[] MagicAll(this ulong[] combinations, ulong number, int shift)
    {
        ulong[] result = new ulong[combinations.Length];

        for (int i = 0; i < combinations.Length; i++)
            result[i] = (combinations[i] * number) >> shift;

        return result;
    }

    private static bool IsDistinct(this ulong[] array)
    {
        return array.Length == new HashSet<ulong>(array).Count;
    }

}