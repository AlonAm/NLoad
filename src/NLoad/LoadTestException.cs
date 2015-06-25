using System;
using System.Diagnostics.CodeAnalysis;

namespace NLoad
{
    [ExcludeFromCodeCoverage]
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