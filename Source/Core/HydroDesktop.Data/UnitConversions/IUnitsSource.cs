using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    interface IUnitsSource
    {
        IEnumerable<Unit> GetUnits();
    }
}