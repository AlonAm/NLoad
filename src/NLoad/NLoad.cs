namespace NLoad
{
    public sealed class NLoad
    {
        public static LoadTestBuilder<T> Test<T>()
            where T : ITest, new()
        {
            return new LoadTestBuilder<T>();
        }

        public static LoadTestBuilder<T> Test<T>(LoadTestConfiguration configuration)
            where T : ITest, new()
        {
            return new LoadTestBuilder<T>(configuration);
        }
    }
}