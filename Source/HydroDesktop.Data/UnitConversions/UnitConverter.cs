using System;
using System.Diagnostics;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.UnitConversions
{
    /// <summary>
    /// Contains methods for units conversion.
    /// </summary>
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
            // Check that given units has same base SI unit
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

        /// <summary>
        /// Ensure that all columns need to unit conversion are present in the current database.
        /// Also it populates units from default database into current database.  
        /// </summary>
        public static void UpdateDefaultUnits()
        {
            var connectionString = Settings.Instance.DataRepositoryConnectionString;
            EnsureColumnsForUnitConversions(connectionString);
            var unitsSource = new DefaultDatabaseUnitsSource();
            UpdateUnits(connectionString, unitsSource);
        }

        /// <summary>
        /// Updates units table from "ODM Controlled Vocabulary"
        /// </summary>
        /// <param name="connectionString">Connection string to database to update. By default it is Settings.Instance.DataRepositoryConnectionString</param>
        public static void UpdateDefaultUnitsFromWeb(string connectionString = null)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = Settings.Instance.DataRepositoryConnectionString;
            }
            EnsureColumnsForUnitConversions(connectionString);
            var unitsSource = new WebUnitsSource();
            UpdateUnits(connectionString, unitsSource);
        }

        #endregion

        #region Private methods

        private static void EnsureColumnsForUnitConversions(string connectionString)
        {
            //todo: implement me
        }

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

        private static void UpdateUnits(string connectionString, IUnitsSource source)
        {
            var unitsRepo = RepositoryFactory.Instance.Get<IUnitsRepository>(DatabaseTypes.SQLite, connectionString);
            foreach (var unit in source.GetUnits())
            {
                if (String.IsNullOrEmpty(unit.Name)) continue;
                if (!unitsRepo.Exists(unit.Name))
                {
                    unitsRepo.AddUnit(unit);
                }
            }
        }

        #endregion
    }
}
