using System;
using System.Diagnostics.CodeAnalysis;

namespace NLoad
{
    [ExcludeFromCodeCoverage]
    public class NLoadException : Exception
    {
        public NLoadException(string message)
            : base(message)
        {
        }

        public NLoadException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}