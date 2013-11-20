using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System.Windows.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace Aggregation_Plugin
{
    public partial class Parameters_form : Form
    {
        AppManager App;
        FeatureSet polygons = new FeatureSet(FeatureType.Polygon);
        List<PolygonData> polygonData = new List<PolygonData>();
        HashSet<String> variables = new HashSet<String>();
        IDataValuesRepository dataValuesRepository = RepositoryFactory.Instance.Get<IDataValuesRepository>();
        DbOperations dbOperations = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
        private readonly IRepositoryManager _repositoryManager = RepositoryFactory.Instance.Get<IRepositoryManager>();
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Parameters_form(AppManager App)
        {
            InitializeComponent();
            this.App = App;
            populatePolygonLayerDropdown();
            populateSites();
            App.Map.MapFrame.SelectionChanged += SelectionChanged;
            PolygonLayerList.SelectedValueChanged += SelectionChanged;
            SiteList.SelectedValueChanged += SelectionChanged;
            VariableList.Text = "Select Variable..."; 
            SiteList.Text = "Select Site..."; 
            PolygonLayerList.Text = "Select Polygon Layer..."; 
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void SelectionChanged(object sender, EventArgs e)
        {
            if (PolygonLayerList.SelectedValue != null && SiteList.SelectedValue != null)
                getPolygons((IMapPolygonLayer)PolygonLayerList.SelectedValue);
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void OK_click_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Output.Text) )
            {
                AggregateData();
            }
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void populatePolygonLayerDropdown()
        {
            var map = (Map)App.Map;
            Dictionary<IMapPolygonLayer, string> layer = new Dictionary<IMapPolygonLayer, string>();

            foreach (var polygonLayer in map.GetAllLayers().OfType<IMapPolygonLayer>().Reverse())
                layer.Add(polygonLayer, polygonLayer.LegendText);

            if (layer.Count > 0)
            {
                PolygonLayerList.DataSource = new BindingSource(layer, null);
                PolygonLayerList.DisplayMember = "Value";
                PolygonLayerList.ValueMember = "Key";
            }
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void populateSites()
        {
            var map = (Map)App.Map;
            Dictionary<IMapPointLayer, string> layer = new Dictionary<IMapPointLayer, string>();

            foreach (var pointLayer in map.GetAllLayers().OfType<IMapPointLayer>().Reverse())
                layer.Add(pointLayer, pointLayer.LegendText);

            if (layer.Count > 0)
            {
                SiteList.DataSource = new BindingSource(layer, null);
                SiteList.DisplayMember = "Value";
                SiteList.ValueMember = "Key";
            }
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void populateVariables()
        {
            polygonData.Clear();

            foreach (IFeature polygon in polygons.Features)
            {
                PolygonData data = new PolygonData();
                data.polygon = polygon;

                IMapPointLayer pointLayer = ((KeyValuePair<IMapPointLayer, string>)SiteList.SelectedItem).Key;
                var features = pointLayer.DataSet.Features;

                foreach (IFeature point in features)
                {
                    if (point.Intersects(polygon))
                    {
                        SiteData siteData = new SiteData();
                        siteData.site = point;

                        foreach (var fld in point.ParentFeatureSet.GetColumns())
                        {
                            var getColumnValue = (Func<string, string>)(column => (point.DataRow[column].ToString()));
                            var strValue = getColumnValue(fld.ColumnName);

                            switch (fld.ColumnName)
                            {
                                case "SiteCode":
                                    siteData.siteCode = strValue;
                                    break;
                                case "VarCode":
                                    siteData.variableCode = strValue;
                                    break;
                                case "VarName":
                                    siteData.variableName = strValue;
                                    break;
                            }
                        }

                        data.sites.Add(siteData);
                        variables.Add(siteData.variableName);
                    }
                }

                polygonData.Add(data);
            }

            if(variables.Count > 0)
                VariableList.DataSource = new BindingSource(variables, null);
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void getPolygons(IMapPolygonLayer polyLayer)
        {
            polygons.Features.Clear();

            if (polyLayer.IsVisible && polyLayer.Selection.Count > 0)
            {
                foreach (var f in polyLayer.Selection.ToFeatureList())
                {
                    polygons.Features.Add(f);
                }

                polygons.Projection = App.Map.Projection;
            }

            populateVariables();
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void AggregateData()
        {
            foreach (var polygon in polygonData)
            {
                foreach(var site in polygon.sites)
                {
                    if((String)VariableList.SelectedItem == site.variableName)
                    {
                        site.variableID = getVariableId(site.variableCode);
                        site.siteID = getSiteId(site.siteCode);

                        getSeriesID(site.siteID, site.variableID, polygon);
                    }
                }

                DataTable averageTable = getAverageTable(polygon);
                Series seriesToSave = getSeriesFromTable(averageTable);
                Theme theme = getThemeFromTable(averageTable);
               // _repositoryManager.SaveSeries(int siteID, int variableID, string methodDescription, string themeName, DataTable dataValues);
                _repositoryManager.SaveSeries(seriesToSave, theme, OverwriteOptions.Append);
            }
        }

        private Theme getThemeFromTable(DataTable averageTable)
        {
            return new Theme();
        }

        private Series getSeriesFromTable(DataTable averageTable)
        {
            Series series = new Series();
            foreach (DataRow row in averageTable.Rows)
            {
                series.AddDataValue((DateTime)row["LocalDateTime"], (Double)row["AVG(DataValues.DataValue)"]);
            }
            /* series.Site;
             *   site.Code
             *   site.SpatialReference.SRSID;
             *   site.SpatialReference.SRSName;
             *   site.LocalProjection.SRSID;
             *   site.LocalProjection.SRSName;
             *   site.LocalProjection.SRSID;
             *   site.LocalProjection.SRSName;
             series.Variable;
             series.Method;
             series.QualityControlLevel;
             series.Source;*/
            return series;
        }



        private int getVariableId(String variableCode)
        {
            var query =
                "SELECT VariableID FROM Variables WHERE VariableCode = "
                + "'" + variableCode + "'";
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        private int getSiteId(String siteCode)
        {
            var query =
                "SELECT SiteID FROM Sites WHERE SiteCode = "
                + "'" + siteCode + "'";
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        private void getSeriesID(int siteID, int variableID, PolygonData polygon)
        {
            var query =
                "SELECT SeriesID FROM DataSeries WHERE SiteID = '"
                + siteID + "'" + 
                "AND VariableID = '"
                + variableID + "'";
            DataTable result = dbOperations.LoadTable(query);

            foreach (DataRow row in result.Rows)
                polygon.dataSeries.Add(Convert.ToInt32(row.ItemArray.First()));
        }

        private DataTable getAverageTable(PolygonData polygon)
        {
            var query =
                "SELECT DataValues.LocalDateTime, AVG(DataValues.DataValue) FROM DataValues"
                + " LEFT JOIN DataSeries ON DataValues.SeriesID == DataSeries.SeriesID"
                + " LEFT JOIN Variables ON DataSeries.VariableID == Variables.VariableID"
                + " WHERE DataValues.SeriesID IN ({0}) AND DataValues.DataValue != Variables.NoDataValue"
                + " GROUP BY DataValues.LocalDateTime";
            var formatted = String.Format(query, String.Join(",", polygon.dataSeries.ToArray()));
            DataTable result = dbOperations.LoadTable(formatted);
            return result;
        }

    }
}
