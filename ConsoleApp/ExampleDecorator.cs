namespace ConsoleApp;

using SourceGenerators;

public class ExampleDecorator : IDecorator
{
    public void OnStart()
    {
        Console.WriteLine("OnStart");
    }

    public void OnException(Exception ex)
    {
        Console.WriteLine($"OnException {ex.Message}");
    }

    public void OnEnd()
    {
        Console.WriteLine("OnEnd");
    }
}
