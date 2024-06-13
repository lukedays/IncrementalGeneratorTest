namespace ConsoleApp;

using SourceGenerator;

public class ExampleDecorator : IDecorator
{
    public void OnEntry()
    {
        Console.WriteLine("OnEntry");
    }

    public void OnException(Exception ex)
    {
        Console.WriteLine($"OnException {ex.Message}");
    }

    public void OnExit()
    {
        Console.WriteLine("OnExit");
    }
}
