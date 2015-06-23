namespace NLoad
{
    public class NLoad
    {
        public static LoadTestBuilder<T> Test<T>()
            where T : ITest, new()
        {
            return new LoadTestBuilder<T>();
        }
    }
}