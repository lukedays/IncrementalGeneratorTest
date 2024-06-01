namespace ConsoleApp;

using SourceGenerators;

public partial class UserClass
{
    public static void Main()
    {
        // Testing cache
        Console.WriteLine(MethodACached(1, 1));
        Console.WriteLine(MethodACached(1, 1));
        GenerateCachedMethodService.Cache.Clear();
        Console.WriteLine(MethodACached(1, 1));
        Console.WriteLine(MethodACached(1, 1));
        Console.WriteLine(MethodACached(1, 2));

        // Testing decorator
        Console.WriteLine(MethodB("a"));
        Console.WriteLine(MethodC("a"));
    }

    [GenerateCachedMethod]
    public static double MethodA(double a, double b)
    {
        Console.Write("Not cached ");
        return a + b;
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static string MethodBInner(string a)
    {
        Console.WriteLine("Implementation");
        return $"Call returned with parameter {a}";
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static string MethodCInner(string a)
    {
        throw new Exception("Oops");
    }
}
