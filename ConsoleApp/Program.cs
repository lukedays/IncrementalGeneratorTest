namespace ConsoleApp;

using SourceGenerators;

public partial class UserClass
{
    public static void Main()
    {
        Console.WriteLine(FuncACached(1, 1));
        Console.WriteLine(FuncACached(1, 1));
        GenerateCachedMethodService.Cache.Clear();
        Console.WriteLine(FuncACached(1, 1));
        Console.WriteLine(FuncACached(1, 1));
        Console.WriteLine(FuncACached(1, 2));

        Console.WriteLine(FuncBCached(1));
        Console.WriteLine(FuncBCached(2));
        Console.WriteLine(FuncBCached(1));
    }

    [GenerateCachedMethod]
    public static double FuncA(double a, double b)
    {
        Console.Write("Not cached ");
        return a + b;
    }

    [GenerateCachedMethod]
    public static string FuncB(double a)
    {
        Console.Write("Not cached ");
        return a.ToString();
    }
}
