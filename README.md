# NLoad
A simple and friendly load testing framework for .NET

[![NuGet downloads](https://img.shields.io/nuget/dt/NLoad.svg)](https://www.nuget.org/packages/NLoad)
[![Version](https://img.shields.io/nuget/v/NLoad.svg)](https://www.nuget.org/packages/NLoad) 
[![AppVeyor](https://img.shields.io/appveyor/ci/AlonAmsalem/nload/master.svg)](https://ci.appveyor.com/project/AlonAmsalem/nload/branch/master)

## Usage

Implement a test class:

```csharp
public class TestRun : ITestRun
{
  public void Initialize()
  {
    // Your test initialization code here (For example, create a WCF client)
  }
  
  public void Execute()
  {
    // Your code here
  }
}
```
Create and run a load test:
```csharp
var loadTestBuilder = new LoadTestBuilder<TestRun>();

var loadTest = loadTestBuilder
                  .WithNumberOfThreads(numberOfThreads)
                  .WithDurationOf(TimeSpan.Zero)
                  .WithDeleyBetweenThreadStart(TimeSpan.Zero)
                  .OnCurrentThroughput((sender, throughput) => Console.WriteLine(throughput))
                  .Build();

var result = loadTest.Run();
```

## Installing NLoad
NLoad can be installed via [NuGet](http://www.nuget.org/packages/NLoad)
```
Install-Package NLoad
```

## License
NLoad is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
