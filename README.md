# Incremental Source Generator Test

Testing the `IIncrementalGenerator` source generator API introduced in .NET 6 by generating cached methods.

### How to use

Clone the project. Build profiles: `ConsoleApp` for the target code, and `Generator` for debugging the source generator (must have .NET Compiler SDK installed as per [this guide](https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022)). Included a `PostsharpApp` alternative for comparison.

### Known bugs

Unfortunately VS 2022 (v. 17.10.0) [needs to be restarted](https://github.com/dotnet/roslyn/issues/50451) for Intellisense to pick up changes in the source generator project - so I found the [Source Generators Auto Update VS extension](https://marketplace.visualstudio.com/items?itemName=AlexanderGayko.AutoUpdateAssemblyName&ssr=false#review-details) to fix this. Changes in the target project work normally.

### GenerateCachedFunction attribute

The `GenerateCachedFunction` is a attribute-based source generator that automatically generates cached versions of methods in your C# code. It aims to improve performance by caching method results based on specified expiration times.