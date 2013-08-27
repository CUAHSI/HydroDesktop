using System.Collections.Generic;
using HydroDesktop.Data.ODMCVServiceClient;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    class WebUnitsSource : IUnitsSource
    {
        #region Implementation of IUnitsSource

        public IEnumerable<Unit> GetUnits()
        {
            var client = ODMCVServiceClientFactory.Instance.GetODMCVServiceClient();
            return client.GetUnits();
        }

        #endregion
    }
}