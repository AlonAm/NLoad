using System.Threading;

namespace NLoad
{
    public class LoadTestContext
    {
        public ManualResetEvent StartEvent { get; set; }

        public ManualResetEvent QuitEvent { get; set; }
    }
}