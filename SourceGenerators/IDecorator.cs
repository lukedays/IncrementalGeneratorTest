namespace SourceGenerators;

public interface IDecorator
{
    public void OnEntry();

    public void OnException(Exception ex);

    public void OnExit();
}
