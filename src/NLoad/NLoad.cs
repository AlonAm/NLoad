using System;

namespace NLoad
{
    public sealed class NLoad
    {
        public static ILoadTestBuilder Test()
        {
            return new LoadTestBuilder();
        }

        public static ILoadTestBuilder Test(LoadTestConfiguration configuration)
        {
            return new LoadTestBuilder(configuration);
        }

        public static ILoadTestBuilder Test<T>()
            where T : ITest, new()
        {
            return Test(typeof(T));
        }

        public static ILoadTestBuilder Test<T>(LoadTestConfiguration configuration)
            where T : ITest, new()
        {
            return Test(typeof(T), configuration);
        }

        public static ILoadTestBuilder Test(Type testType)
        {
            var builder = Test();

            builder.OfType(testType);

            return builder;
        }

        public static ILoadTestBuilder Test(Type testType, LoadTestConfiguration configuration)
        {
            var builder = Test(configuration);

            builder.OfType(testType);

            return builder;
        }
    }
}