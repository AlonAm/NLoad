using System;

namespace NLoad
{
    public class LoadTestException : Exception
    {
        public LoadTestException(string message)
            : base(message)
        {
        }

        public LoadTestException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}