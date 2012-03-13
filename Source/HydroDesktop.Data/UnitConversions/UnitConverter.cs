using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    public static class UnitConverter
    {
        #region Public methods

        /// <summary>
        /// Shows that one unit can be converted to another unit
        /// </summary>
        /// <param name="unitA">Source unit</param>
        /// <param name="unitB"></param>
        /// <returns></returns>
        public static bool CanConvertUnits(Unit unitA, Unit unitB)
        {
            // Check that given units hase same base SI unit
            if (unitA.Dimension != unitB.Dimension ||
                unitA.UnitsType != unitB.UnitsType)
            {
                return false;
            }

            // Check for Conversion factor and offset
            if (unitA.ConversionFactorToSI.HasValue &&
                unitA.OffsetToSI.HasValue &&
                unitB.ConversionFactorToSI.HasValue &&
                unitB.OffsetToSI.HasValue)
            {
                return true;
            }

            // Check for conversion via UnitConversions table
            return RepositoryFactory.Instance.Get<IUnitConversionsRepository>().ExistsConversion(unitA, unitB);
        }

        /// <summary>
        /// Convert value from one unit to another
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <param name="originalUnit">Unit of original value</param>
        /// <param name="newUnit">New Unit</param>
        /// <returns>Converted value.</returns>
        public static  double ConvertValue (double originalValue, Unit originalUnit, Unit newUnit)
        {
            var conversionFunc = GetConversionFunc(originalUnit, newUnit);
            return conversionFunc(originalValue);
        }

        /// <summary>
        /// Convert all values in series to new unit
        /// </summary>
        /// <param name="originalSeries">Original series</param>
        /// <param name="newUnit">New unit</param>
        /// <returns>New series with converted values.</returns>
        public static Series ConvertSeries (Series originalSeries, Unit newUnit)
        {
            var conversionFunc = GetConversionFunc(originalSeries.Variable.VariableUnit, newUnit);
            var series = new Series(originalSeries, false) {Variable = (Variable) originalSeries.Variable.Clone()};
            series.Variable.VariableUnit = newUnit;
            foreach (var dataValue in originalSeries.DataValueList)
            {
                var value = conversionFunc(dataValue.Value);
                series.AddDataValue(dataValue.LocalDateTime, value, dataValue.UTCOffset, dataValue.Qualifier);
            }

            return series;
        }

        public static void UpdateDefaultUnits()
        {
            throw new NotImplementedException();
        }

        public static void UpdateDefaultUnitsFromWeb(string connectionString = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private static Func<double, double> GetConversionFunc(Unit originalUnit, Unit newUnit)
        {
            var conversion = RepositoryFactory.Instance.Get<IUnitConversionsRepository>().GetConversion(originalUnit,
                                                                                                        newUnit);
            if (conversion != null)
            {
                // ConversionFactor * (x + Offset)
                return x => conversion.ConversionFactor*(x + conversion.Offset);
            }

            return delegate(double x)
                       {
                           // Convert to SI
                           // f^-1(x) = ConversionFactorToSI * (x + OffsetToSI)
                           var originalInSI = originalUnit.ConversionFactorToSI*(x + originalUnit.OffsetToSI);

                           // Convert to New Unit
                           // f(x) = 1/ConversionFactorToSI *x - OffsetToSI
                           var res = 1 / newUnit.ConversionFactorToSI * originalInSI - newUnit.OffsetToSI;

                           Debug.Assert(res != null, "res != null");
                           return (double) res;
                       };
        }

        #endregion
    }
}
