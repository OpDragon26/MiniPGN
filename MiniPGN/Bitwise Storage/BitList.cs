namespace MiniPGN.Bitwise_Storage;

public class BitList
{
    // the purpose of this class is to store information ignoring byte boundaries
    
    private readonly List<byte> InnerList = [];
    private int LastFilled => (int)(Count % 8); // relevant bits in the last byte
    public ulong Count;

    public void AddBits(ulong value, int bits)
    {
        for (int b = bits - 1; b >= 0; b--)
        {
            ulong relevant = value >> b;
            AddBit((byte)relevant);
        }
    }

    // adds the most relevant bit to the 
    public void AddBit(byte bit)
    {
        if (LastFilled == 0)
            InnerList.Add(0);
        Count++;
        
        byte b = (byte)(bit & 1);
        int push = LastFilled == 0 ? 0 : 8 - LastFilled;
        
        InnerList[^1] |= (byte)(b << push);
    }
    
    public override string ToString()
    {
        return ConvertToBase(16);
    }

    public string ConvertToBase(int b)
    {
        int totalWidth = Convert.ToString(byte.MaxValue, b).Length;
        return string.Join(" ", InnerList.Select(x => Convert.ToString(x, b).PadLeft(totalWidth, '0')));
    }

    public byte[] ToByteArray()
    {
        return InnerList.ToArray();
    }

    public void Clear()
    {
        InnerList.Clear();
        Count = 0;
    }
}