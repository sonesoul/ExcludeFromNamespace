# Exclude From Namespace

A lightweight Visual Studio extension that prevents unwanted directory names from appearing in C# namespaces.

## Features
- Automatically fixes namespaces when new C# files are added
- Configurable excluded directory name
- Enable/disable support through settings
- Lightweight and focused
- Works with Visual Studio's native extension system

## Why?
By default, Visual Studio includes folder names when generating namespaces. \
Creating a file at `MyProject/src/Utils/` results in:
```csharp
namespace MyProject.src.Utils
{
  internal class MyClass { }
}
```
However, folders like `src` or `Source` usually should not be part of the namespace.

**Exclude From Namespace** automatically removes configured directory names from generated namespaces. \
Before:
```
namespace MyProject.src.Utils
```
After:
```
namespace MyProject.Utils
```
No heavy IDE extensions. No project modifications. Just a small tool that fixes only **one** annoying workflow problem.
