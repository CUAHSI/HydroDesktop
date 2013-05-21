using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Data.ODMCVServiceClient
{
    public interface IODMCVServiceClient
    {
        IEnumerable<Unit> GetUnits();
    }
}
