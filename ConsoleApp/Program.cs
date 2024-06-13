namespace ConsoleApp;

using SourceGenerator;

public class Entry
{
    public static async Task Main()
    {
        await UserClass<List<string>>.TestAll();
    }
}

public partial class UserClass<X>
    where X : IList<string>
{
    public static async Task TestAll()
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
        Console.WriteLine(DecoratorException());

        Console.WriteLine($"Decorator test, {nameof(DecoratorVoid)}");
        DecoratorVoid();

        Console.WriteLine($"Decorator test, {nameof(DecoratorTask)}");
        await DecoratorTask();
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
    private static void DecoratorVoidInner()
    {
        Console.WriteLine("Void");
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static async Task DecoratorTaskInner()
    {
        await Task.Delay(1);
        Console.WriteLine("Task");
    }

    [GenerateDecoratedMethod(nameof(ExampleDecorator))]
    private static string DecoratorExceptionInner()
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
