namespace ConsoleApp;

using SourceGenerators;

public class Entry
{
    public static void Main()
    {
        UserClass<List<string>>.TestAll();
    }
}

public partial class UserClass<X>
    where X : IList<string>
{
    public static void TestAll()
    {
        Console.WriteLine($"Cache test, {nameof(TestCached)}");
        Console.WriteLine(TestCached(1, 1));
        Console.WriteLine(TestCached(1, 1));
        GenerateCachedMethodService.Cache.Clear();
        Console.WriteLine(TestCached(1, 1));
        Console.WriteLine(TestCached(1, 1));
        Console.WriteLine(TestCached(1, 2));

        Console.WriteLine($"Cache test, {nameof(GenericListCached)}");
        List<Rec> recList = [new Rec("a")];
        Console.WriteLine(GenericListCached(recList));
        Console.WriteLine(GenericListCached(recList));
        recList.Add(new Rec("b"));
        Console.WriteLine(GenericListCached(recList));

        Console.WriteLine($"Cache test, {nameof(ConsoleApp2.ConsoleApp2.SumOtherCached)}");
        Console.WriteLine(ConsoleApp2.ConsoleApp2.SumOtherCached(1, 1));
        GenerateCachedMethodService.Cache.Clear();
        Console.WriteLine(ConsoleApp2.ConsoleApp2.SumOtherCached(1, 1));
        Console.WriteLine(ConsoleApp2.ConsoleApp2.SumOtherCached(1, 1));

        Console.WriteLine($"Decorator test, {nameof(Decorator)}");
        Console.WriteLine(Decorator("a"));

        Console.WriteLine($"Decorator test, {nameof(DecoratorException)}");
        Console.WriteLine(DecoratorException("a"));
    }

    public record Rec(string a);

    [GenerateCachedMethod]
    public static double Test(
        double a,
        int b,
        string c = "a",
        List<string>? x = default,
        DateTime y = default
    )
    {
        Console.Write("Not cached ");
        return a + b;
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static string DecoratorInner(string a)
    {
        Console.WriteLine("Implementation");
        return $"Call returned with parameter {a}";
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static string DecoratorExceptionInner(string a)
    {
        throw new Exception("Oops");
    }

    [GenerateCachedMethod]
    public static double GenericList<T>(List<T> a)
        where T : class
    {
        Console.Write("Not cached ");
        return a.Count;
    }
}
