using System;
using System.Threading;

namespace NLoad.App.Tests
{
    [LoadTest]
    public sealed class InMemoryTest1 : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1);

            return TestResult.Success;
        }
    }

    [LoadTest]
    public sealed class InMemoryTest10 : ITest
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

    [LoadTest]
    public sealed class InMemoryTest100 : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(100);

            return TestResult.Success;
        }
    }

    [LoadTest]
    public sealed class InMemoryTest1000 : ITest
    {
        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            Thread.Sleep(1000);

            return TestResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LoadTestAttribute : Attribute
    {
    }
}