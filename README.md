# Incremental Source Generator Test

Testing the `IIncrementalGenerator` source generator API introduced in .NET 6 by generating cached methods.

### How to use

Clone the project. Build profiles: `ConsoleApp` for the target code, and `Generator` for debugging the source generator (must have .NET Compiler SDK installed as per [this guide](https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022)). Included a `PostsharpApp` alternative for comparison.

### Known bugs

Unfortunately VS 2022 (v. 17.10.0) [needs to be restarted](https://github.com/dotnet/roslyn/issues/50451) for Intellisense to pick up changes in the source generator project. Also, debug breakpoints don't hit on the source generated files, and intellisense doesn't pick up the references.
Workarounds:
- Generate files in the project folder as [described here](https://github.com/dotnet/roslyn/issues/44093), but this also generates error CS0121 - ambigous reference
- [Source Generators Auto Update VS extension](https://marketplace.visualstudio.com/items?itemName=AlexanderGayko.AutoUpdateAssemblyName&ssr=false#review-details)

### GenerateCachedMethod attribute

Automatically generates cached versions of methods, to improve performance by caching method results based on specified expiration times.

### GenerateDecoratorMethod attribute

Creates actions to run before, after and on exceptions.