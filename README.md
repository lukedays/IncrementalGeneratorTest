# Incremental Generator Test

Testing the `IIncrementalGenerator` source generator API introduced in .NET 6 for generating cached methods.
Build profiles: `ConsoleApp` for the final code, and `Generator` for debugging the source generator (must have .NET Compiler SDK installed as per [this guide](https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022))

## Example: GenerateCachedFunction attribute

The `GenerateCachedFunction` source generator is a attribute-based generator that automatically generates cached versions of methods in your C# code. It aims to improve performance by caching method results based on specified expiration times.