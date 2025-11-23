using MiniPGN.Bitwise_Storage;

BitList TestList = new BitList();
TestList.AddBits(0b001111000101011UL, 24);

Console.WriteLine(TestList.ConvertToBase(2));