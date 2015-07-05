using System.Net;

namespace NLoad.App.Features.RunLoadTest
{
    class HttpRequestTest:ITest
    {
        readonly WebClient _webClient = new WebClient();

        public void Initialize()
        {
        }

        public TestResult Execute()
        {
            var str = _webClient.DownloadString("http://localhost:49276/");

            return str.Contains("Test") ? TestResult.Success : TestResult.Failed;
        }
    }
}
