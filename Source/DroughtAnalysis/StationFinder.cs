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
    public class StationFinder
    {
        public StationFinder(IMap map)
        {
            MainMap = map;
        }

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

            RepositoryManagerSQL repo = new RepositoryManagerSQL(HydroDesktop.Interfaces.DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString);
            IList<Site> suitableSites = repo.GetSitesWithBothVariables(tmpVa, pcpVa);
            return suitableSites;
        }

        private DataTable GetVariableTable(string filter)
        {
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

        public Variable GetTemperatureVariable()
        {
            DataTable tab = GetVariableTable("VariableName LIKE 'temperatur%' AND DataType = 'Average'");
            if (tab.Rows.Count > 0)
                return VariableFromDataRow(tab.Rows[0]);
            else
                return null;
        }

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
