using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using HydroDesktop.Interfaces.ObjectModel;
using System.Data;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace DroughtAnalysis
{
    /// <summary>
    /// Finds a station that has all required variables
    /// </summary>
    public class StationFinder
    {
        /// <summary>
        /// Creates a new instance of the station finder class
        /// </summary>
        /// <param name="map">The map of the application</param>
        public StationFinder(IMap map)
        {
            MainMap = map;
        }

        /// <summary>
        /// Gets or sets the main map where stations are found
        /// </summary>
        public IMap MainMap { get; set; }
        
        /// <summary>
        /// Finds all stations that measure temperature and precipitation
        /// </summary>
        /// <returns></returns>
        public IList<Site> FindSuitableStations()
        {
            Variable tmpVa = GetTemperatureVariable();
            Variable pcpVa = GetPrecipitationVariable();
            if (tmpVa == null || pcpVa == null) return new List<Site>();

            var repo = RepositoryFactory.Instance.Get<IDataSeriesRepository>(DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString);
            //IList<Site> suitableSites = repo.GetSitesWithBothVariables(tmpVa, pcpVa);
            //var manager = new HydroDesktop.Database.
            //return suitableSites;
            //TODO: need to implement this method in a suitable location
            throw new NotImplementedException();
        }

        private DataTable GetVariableTable(string filter)
        {
            //finds the variables that fulfill the filter conditions
            string sqlstr = "SELECT VariableID, VariableName, VariableCode, DataType, ValueType, Speciation, SampleMedium, " +
                "TimeSupport, GeneralCategory, NoDataValue, " +
                "units1.UnitsName as 'VariableUnitsName', units2.UnitsName as 'TimeUnitsName' FROM Variables " +
                "INNER JOIN Units units1 ON Variables.VariableUnitsID = units1.UnitsID " +
                "INNER JOIN Units units2 ON Variables.TimeUnitsID = units2.UnitsID WHERE " +
                filter;

            DbOperations db = new DbOperations(HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
            return db.LoadTable(sqlstr);
        }

        private Variable VariableFromDataRow(DataRow row)
        {
            Unit timeUnit = Unit.UnknownTimeUnit;
            timeUnit.Name = (string)row["TimeUnitsName"];

            Unit variableUnit = Unit.Unknown;
            variableUnit.Abbreviation = (string)row["VariableUnitsName"];

            Variable v = new Variable();
            v.Id = (long)row["VariableID"];
            v.Name = (string)row["VariableName"];
            v.Code = (string)row["VariableCode"];
            v.DataType = (string)row["DataType"];
            v.ValueType = (string)row["ValueType"];
            v.Speciation = row["Speciation"] == DBNull.Value ? null : (string)row["Speciation"];
            v.SampleMedium = row["SampleMedium"] == DBNull.Value ? null : (string)row["SampleMedium"];
            v.TimeSupport = row["TimeSupport"] == DBNull.Value ? 0 : (double)row["TimeSupport"];
            v.VariableUnit = variableUnit;
            v.TimeUnit = timeUnit;
            v.GeneralCategory = row["GeneralCategory"] == DBNull.Value ? null : (string)row["GeneralCategory"];
            v.NoDataValue = (double)row["NoDataValue"];
            return v;
        }

        /// <summary>
        /// Gets the variable object that measures temperature
        /// </summary>
        /// <returns>The variable object</returns>
        public Variable GetTemperatureVariable()
        {
            DataTable tab = GetVariableTable("VariableName LIKE 'temperatur%' AND DataType = 'Average'");
            if (tab.Rows.Count > 0)
                return VariableFromDataRow(tab.Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// Gets the variable object that measures precipitation
        /// </summary>
        /// <returns>The precipitation variable object</returns>
        public Variable GetPrecipitationVariable()
        {
            DataTable tab = GetVariableTable("VariableName LIKE 'precip%'");
            if (tab.Rows.Count > 0)
                return VariableFromDataRow(tab.Rows[0]);
            else
                return null;
        }
    }
}
