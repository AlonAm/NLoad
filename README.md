# NLoad

[![NuGet](https://img.shields.io/nuget/v/NLoad.svg?maxAge=2592000)]()
[![Build status](https://ci.appveyor.com/api/projects/status/beofqq7vuegb4ax7?svg=true)](https://ci.appveyor.com/project/AlonAmsalem/nload)

NLoad is a simple and easy to use load testing framework for .NET that helps testing Websites, WCF Services and small bits of code to identify and eliminate bottlenecks and concurrency issues.

### Installation
To install NLoad via [NuGet](http://www.nuget.org/packages/NLoad), run the following command in the Package Manager Console
```
Install-Package NLoad
```

### Getting Started

Implement a class that inherits the [ITest](https://github.com/AlonAm/NLoad/blob/master/src/NLoad/Scenario/ITest.cs) interface.


For example:

```csharp
public class MyTest : ITest
{
  public void Initialize()
  {
    // Initialize the test, e.g., create a WCF client, load files into memory, etc.
  }

  public TestResult Execute()
  {
    // Send an http request, invoke a WCF service or whatever you want to load test.
    
    return TestResult.Success; // or TestResult.Failure
  }
}
```
This class will be created and initialized multiple times during the load test.

Create and configure a load test:

```csharp
var loadTest = NLoad.Test<MyTest>()
		  .WithNumberOfThreads(5)
		  .WithDurationOf(TimeSpan.FromMinutes(5))
		  .OnHeartbeat((s, e) => Console.WriteLine(e.Throughput))
		.Build();
```

Run it

```csharp
var result = loadTest.Run();
```

### Load Test Result

* Total Errors
* Total Runtime
* Total Iterations
* Min/Max/Average Throughput
* Min/Max/Average Response Time

### Events

Event| Description        
-----|------------
Heartbeat | Fired every one second
Starting | Fired when the load test is starting
Finished | Fired when the load test completes
Aborted | Fired when the load test is aborted

### License

Apache 2.0
