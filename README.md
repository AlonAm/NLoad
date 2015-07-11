#NLoad

[![NuGet](https://img.shields.io/nuget/dt/NLoad.svg?style=flat-square)](https://www.nuget.org/packages/NLoad)
[![NuGet](https://img.shields.io/nuget/v/NLoad.svg?style=flat-square)](https://www.nuget.org/packages/NLoad)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/AlonAmsalem/nload/master.svg?style=flat-square)](https://ci.appveyor.com/project/AlonAmsalem/nload/branch/master)

NLoad is a simple and easy to use load testing framework for .NET, Intended for load testing your code and figuring out how many concurrent operations your code can handle.

NLoad is used for load testing websites, WCF services, CPU intensive algorithms or small bits of code to identify bottlenecks before letting real users in.

Using NLoad is as simple as
```csharp
var loadTest = NLoad.Test<MyTest>()
                      .WithNumberOfThreads(100)
                      .WithDurationOf(TimeSpan.FromMinutes(5))
                      .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(50))
                      .OnHeartbeat((s, e) => Console.WriteLine(e.Throughput))
                    .Build();

var result = loadTest.Run();
```

For more information see [Getting Started](https://github.com/NLoad/NLoad/wiki/Getting-Started)

### Installation
To install NLoad via [NuGet](http://www.nuget.org/packages/NLoad), run the following command in the Package Manager Console
```
Install-Package NLoad
```
