using System;
using System.Data;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroDesktop.Interfaces;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace HydroDesktop.ExportToCSV
{
    /// <summary>
    /// The main data export plugin class
    /// </summary>
    public class MySQLExporterPlugin : Extension
    {
        #region Variables

        //reference to the main application and it's UI items
        /// <summary>
        /// The key of the "Table" ribbon tab
        /// </summary>
        private readonly string TableTabKey = SharedConstants.TableRootKey;

        /// <summary>
        /// The name of the "Data Export" panel on the table ribbon
        /// </summary>
        private const string _panelName = "Data Export";

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }

        #endregion
		
        #region IPlugin Members

        /// <summary>
        /// activate the data export plugin
        /// </summary>
        public override void Activate()
        {
            //Add "DataExport" button to the new "Data Export" Panel in "Data" ribbon tab
            var dataExportBtn = new SimpleActionItem("MySQL Export", dataExportBtn_Click)
                                    {
                                        RootKey = TableTabKey,
                                        ToolTipText = "Export Time Series Data",
                                        GroupCaption = _panelName
                                    };
            App.HeaderControl.Add(dataExportBtn);

            base.Activate();
        }

        #endregion

        #region Event Handlers

        void dataExportBtn_Click(object sender, EventArgs e)
        {
            var repository = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
            IList<Series> allSeries = repository.GetAll();


            //export all sites
            var siteRepo = RepositoryFactory.Instance.Get<ISitesRepository>();
            DataTable sdt = siteRepo.AsDataTable();
            using (StreamWriter writer = new StreamWriter("J:\\sites.txt"))
            {
                foreach (DataRow r in sdt.Rows)
                {
                    string insertStr = string.Format("INSERT INTO sites (SiteID, SiteCode, SiteName, Latitude, Longitude, LatLongDatumID, SiteType, Elevation_m, VerticalDatum)" +
                        " VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                        r["SiteID"], r["SiteCode"], r["SiteName"], r["Latitude"], r["Longitude"], 3, "Atmosphere", r["Elevation_m"], "MSL");
                        writer.WriteLine(insertStr);
                }
                writer.Flush();
            }

            //export all variables
            var variableRepo = RepositoryFactory.Instance.Get<IVariablesRepository>();
            IList<Variable> vars = variableRepo.GetAll();
            using (StreamWriter writer = new StreamWriter("J:\\variables.txt"))
            {
                foreach (Variable v in vars)
                {
                    int unitID = 96;
                    if (v.VariableUnit.Name.ToLower().Contains("millimeter"))
                    {
                        unitID = 54;
                    }
                    string insertStr = string.Format("INSERT INTO variables(VariableID, VariableCode, VariableName, Speciation, VariableunitsID, SampleMedium, " +
                        " ValueType, IsRegular, TimeSupport, TimeunitsID, DataType, GeneralCategory, NoDataValue ) VALUES (" +
                        " '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
                        v.Id, v.Code, v.Name, v.Speciation, unitID, "Air", "Field Observation", 1, 1, 96, v.DataType, v.GeneralCategory, v.NoDataValue);
                    writer.WriteLine(insertStr);
                }
                writer.Flush();
            }

            string fn = "series.txt";
            using (StreamWriter writer = new StreamWriter("J:\\" + fn))
            {
                foreach(Series s in allSeries)
                {          
                    var dvRepo = RepositoryFactory.Instance.Get<IDataValuesRepository>();
                    DataTable dt = dvRepo.GetTableForExport(s.Id);
                    foreach (DataRow r in dt.Rows)
                    {
                        string insertStr = string.Format("INSERT INTO datavalues (DataValue, LocalDateTime, UTCOffset, DateTimeUTC, SiteID, VariableID, CensorCode, MethodID, SourceID, QualityControlLevelID) VALUES " +
                        " ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '1', '1', '1');",
                        r["DataValue"], 
                        (Convert.ToDateTime(r["LocalDateTime"])).ToString("s", CultureInfo.InvariantCulture), 
                        r["UTCOffset"], 
                        (Convert.ToDateTime(r["LocalDateTime"])).ToString("s", CultureInfo.InvariantCulture),
                        r["SiteID"], r["VariableID"], r["CensorCode"]);

                        writer.WriteLine(insertStr);
                    }               
                }
                writer.Flush();
            }
        }

        #endregion
    }
}
