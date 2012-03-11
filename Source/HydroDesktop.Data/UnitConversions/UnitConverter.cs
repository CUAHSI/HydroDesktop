using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    public static class UnitConverter
    {
        public static bool CanConvertUnits(Unit unitA, Unit unitB)
        {
            throw new NotImplementedException();
        }

        public static  double ConvertValue (double originalValue, Unit originalUnit, Unit newUnit)
        {
            throw new NotImplementedException();
        }

        public static Series ConvertSeries (Series originalSeries, Unit newUnit)
        {
            throw new NotImplementedException();
        }

        public static void UpdateDefaultUnits()
        {
            throw new NotImplementedException();
        }

        public static void UpdateDefaultUnitsFromWeb(string connectionString = null)
        {
            throw new NotImplementedException();
        }
    }
}
