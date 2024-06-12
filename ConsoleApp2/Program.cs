namespace ConsoleApp2;

using SourceGenerators;

public partial class ConsoleApp2
{
    public static void Main() { }

    [GenerateCachedMethod]
    public static double SumOther(double a, double b)
    {
        Console.Write("Not cached ");
        return a + b;
    }
}
