using System;
using System.Threading;

namespace NLoad.App.Tests
{
    [LoadTest]
    internal sealed class InMemoryTest : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(10);

            return TestResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class LoadTestAttribute : Attribute
    {
        public LoadTestAttribute()
        {
        }
    }
}