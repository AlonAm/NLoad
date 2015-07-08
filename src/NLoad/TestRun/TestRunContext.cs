using System.Threading;

namespace NLoad
{
    public class TestRunContext
    {
        public ManualResetEvent StartEvent { get; set; }
        public ManualResetEvent QuitEvent { get; set; }
    }
}