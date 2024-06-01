using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;

namespace PostsharpApp;

public class UserClass
{
    public static void Main()
    {
        CachingServices.DefaultBackend = new MemoryCachingBackend();

        // Testing cache
        Console.WriteLine(MethodA(1, 1));
        Console.WriteLine(MethodA(1, 1));
        CachingServices.DefaultBackend.Clear();
        Console.WriteLine(MethodA(1, 1));
        Console.WriteLine(MethodA(1, 1));
        Console.WriteLine(MethodA(1, 2));

        // Testing decorator
        Console.WriteLine(MethodB("a"));
        Console.WriteLine(MethodC("a"));
    }

    [Cache]
    public static double MethodA(double a, double b)
    {
        Console.Write("Not cached ");
        return a + b;
    }

    [LoggingAspect]
    private static string MethodB(string a)
    {
        Console.WriteLine("Implementation");
        return $"Call returned with parameter {a}";
    }

    [LoggingAspect]
    private static string MethodC(string a)
    {
        throw new Exception("Oops");
    }
}
