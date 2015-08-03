using System.Threading;

namespace NLoad
{
    public class LoadTestContext
    {
        public LoadTestContext()
        {
            StartEvent = new ManualResetEvent(false);
            QuitEvent = new ManualResetEvent(false);
        }

        public ManualResetEvent StartEvent { get; private set; }

        public ManualResetEvent QuitEvent { get; private set; }
    }
}