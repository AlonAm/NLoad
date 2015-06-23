# NLoad
A simple and friendly load testing framework for .NET

[![NuGet downloads](https://img.shields.io/nuget/dt/NLoad.svg)](https://www.nuget.org/packages/NLoad)
[![Version](https://img.shields.io/nuget/v/NLoad.svg)](https://www.nuget.org/packages/NLoad) 
[![AppVeyor](https://img.shields.io/appveyor/ci/AlonAmsalem/nload/master.svg)](https://ci.appveyor.com/project/AlonAmsalem/nload/branch/master)

## Usage

Implement a test class:

```csharp
public class MyTest : ITest
{
  public void Initialize()
  {
    // Create WCF clients, load files, etc.
  }
  
  public void Execute()
  {
    // Send http request, invoke a WCF service or whatever you want to load test.
  }
}
```
Create, configure and run your load test:
```csharp
var loadTest = NLoad.Test<MyTest>()
                      .WithNumberOfThreads(500)
                      .WithDurationOf(TimeSpan.FromMinutes(5))
                      .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
                      .OnCurrentThroughput((s, throughput) => Console.WriteLine(throughput))
                    .Build();

var result = loadTest.Run();
```

## Installation
NLoad can be installed via [NuGet](http://www.nuget.org/packages/NLoad)
```
Install-Package NLoad
```

## License
NLoad is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
