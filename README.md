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
    // Initialize your test, e.g., create a WCF client, load files, etc.
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
                      .OnHeartbeat((sender, heartbeat) => Console.WriteLine(heartbeat.Throughput))
                    .Build();

var result = loadTest.Run();
```

## Installation
To install NLoad via [NuGet](http://www.nuget.org/packages/NLoad), run the following command in the Package Manager Console
```
Install-Package NLoad
```

## License
NLoad is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license file for more information.
