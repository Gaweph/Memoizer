# Memoizer

This is a quick and easy way of adding [Memoization](https://en.wikipedia.org/wiki/Memoization)/Caching to your C# project.

[Memoization](https://en.wikipedia.org/wiki/Memoization) is an optimization technique used primarily to speed up computer programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again.

## Installation

Add the NuGet package to your project.

```powershell
PM> Install-Package Memoizer.NETStandard
```
## Getting Started

Decorate a method with the `[Memoize]` attribute.  

```csharp
[Memoize] // <-- add this
public BigInteger Fib(int n)
{
    if (n < 2) return n;
    return Fib(n - 1) + Fib(n - 2);
}
```

Calls to this method with matching arguments will now be cached meaning only the first call with a unique set or args will execute the internal code.

## Why

There are many use cases where it is appropriate to cache the result of a method call.  A popular use case is a method that returns the Nth [Fibonacci](https://en.wikipedia.org/wiki/Fibonacci) number.  The time taken increases exponentially as you increase N.

## Example

### With Caching
```csharp
[Memoize]
public BigInteger Fib(int n)
{
    if (n < 2) return n;
    return Fib(n - 1) + Fib(n - 2);
}
Fib(5); // 0.003s
Fib(50); // 0.003s
Fib(300); // 0.010s
Fib(1000); // 0.013s
```

### Without Caching
```csharp
// Brute Force method that does not Use Memoization
public BigInteger Fib(int n)
{
    if (n < 2) return n;
    return RecursiveFib(n - 1) + RecursiveFib(n - 2);
}

Fib(5); // 0.003s
Fib(25); // 0.007s
Fib(40); // 8s
Fib(50); // 977s (16 Minutes)
Fib(100); // Come back tomorrow

```

As you can see the use of Caching in this example allows usto actually work out the Fib of 100 in neglegeable time.
