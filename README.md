# Memoizer

This is a quick and easy way of adding [Memoization](https://en.wikipedia.org/wiki/Memoization)/Caching to your C# project.

[![Version](https://img.shields.io/nuget/vpre/Memoizer.NETStandard.svg)](https://www.nuget.org/packages/Memoizer.NETStandard)
[![Downloads](https://img.shields.io/nuget/dt/Memoizer.NETStandard.svg)](https://www.nuget.org/packages/Memoizer.NETStandard)

```csharp
// Result will be cached for the lifetime of this application
[Cache]
public string HelloWorld() => "Hello World!";

// Result will be cached for 500 Milliseconds
[Cache(500)]
public string HelloWorld() => "500 Millisecond Cache";

// Result will be cached for 5 seconds
[Cache(5, Time.Second)]
public string HelloWorld() => "5 Second Cache";

```

Calls to cached method with matching arguments will be cached meaning only the first call with a unique set or args will execute the internal code.

## Installation

Add the NuGet package to your project.

```
PM> Install-Package Memoizer.NETStandard
```


## Why

[Memoization](https://en.wikipedia.org/wiki/Memoization) is an optimization technique used primarily to speed up computer programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again.

## Usage Example

There are many use cases where it is appropriate to cache the result of a method call.  A popular use case is a method that returns the Nth [Fibonacci](https://en.wikipedia.org/wiki/Fibonacci) number.  The time taken increases exponentially as you increase N.

### With Caching
```csharp
[Cache]
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
    return Fib(n - 1) + Fib(n - 2);
}

Fib(5); // 0.003s
Fib(50); // 977s (16 Minutes)
Fib(300); // Come back tomorrow
Fib(1000); // Years!!

```

As you can see the use of Caching in this example allows us to actually work out the Fib of 1000 in neglegeable time.
