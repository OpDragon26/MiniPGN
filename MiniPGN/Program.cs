using MiniPGN.Minimizer;
using Type = MiniPGN.Minimizer.Type;

Console.WriteLine(Encoder.Active.Encode(new(Type.Standard, Metadata.Include)));