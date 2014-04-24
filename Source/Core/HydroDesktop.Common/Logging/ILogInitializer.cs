using System;

namespace HydroDesktop.Common.Logging
{
    interface ILogInitializer : IDisposable
    {
        string Destination { get; }
    }
}