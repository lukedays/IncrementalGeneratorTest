using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;

namespace PostsharpApp;

public class UserClass
{
    public static void Main()
    {
        CachingServices.DefaultBackend = new MemoryCachingBackend();

        Console.WriteLine(FuncA(1, 1));
        Console.WriteLine(FuncA(1, 1));
        Console.WriteLine(FuncA(1, 2));

        Console.WriteLine(FuncB(1));
        Console.WriteLine(FuncB(2));
        Console.WriteLine(FuncB(1));
    }

    [Cache]
    public static double FuncA(double a, double b)
    {
        Console.Write("Not cached ");
        return a + b;
    }

    [Cache]
    public static string FuncB(double a)
    {
        Console.Write("Not cached ");
        return a.ToString();
    }
}
