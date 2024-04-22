namespace TestDIApp;

internal sealed class RandomPrefetchCountGenerator
{
    public ushort Generate()
    {
        return (ushort)new Random().Next(1, 200);
    }
}
