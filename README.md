#NLoad

NLoad is a free and open source load testing framework for .NET used for testing Websites, WCF Services or small bits of code to identify and eliminate bottlenecks at an early stage of development.

### Installation
To install NLoad via [NuGet](http://www.nuget.org/packages/NLoad), run the following command in the Package Manager Console
```
Install-Package NLoad
```

### Getting Started

1. Implement a class that inherits the ITest interface. For example:

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

2. Create and configure a load test:

```csharp
var loadTest = NLoad.Test<MyTest>()
		  .WithNumberOfThreads(5)
		  .WithDurationOf(TimeSpan.FromMinutes(5))
		  .OnHeartbeat((s, e) => Console.WriteLine(e.Throughput))
		.Build();
```

3. Run it

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
