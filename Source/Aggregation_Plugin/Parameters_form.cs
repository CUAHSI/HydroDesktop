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
using HydroDesktop.Interfaces.ObjectModel;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using System.Collections;

namespace Aggregation_Plugin
{
    public partial class Parameters_form : Form
    {
        AppManager App;
        FeatureSet polygons = new FeatureSet(FeatureType.Polygon);
        IFeatureSet sitesPoints = new FeatureSet(FeatureType.Point);

        List<PolygonData> polygonData = new List<PolygonData>();
        HashSet<String> variables = new HashSet<String>();
        IUnitsRepository UnitsRepository = RepositoryFactory.Instance.Get<IUnitsRepository>();
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
            //App.Map.MapFrame.SelectionChanged += SelectionChanged;
            PolygonLayerList.SelectedValueChanged += PolySelectionChanged;
            SiteList.SelectedValueChanged += SiteSelectionChanged;

            PolygonLayerList.SelectedIndex = -1;
            SiteList.SelectedIndex = -1;
            //SiteList.DataSource = new List<string>();
            VariableList.SelectedIndex = -1;
            //VariableList.DataSource = new List<string>();

        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void PolySelectionChanged(object sender, EventArgs e)
        {
            if (PolygonLayerList.SelectedIndex == -1)
            {
                return;
            }

            SiteList.SelectedIndex = -1;
            VariableList.SelectedIndex = -1;
            variables.Clear();

            getPolygons((IMapPolygonLayer)PolygonLayerList.SelectedValue);
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void SiteSelectionChanged(object sender, EventArgs e)
        {
            VariableList.SelectedIndex = -1;
            variables.Clear();

            if (PolygonLayerList.SelectedValue != null && SiteList.SelectedValue != null)
            {
                populateVariables();
                VariableList.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void OK_Click(object sender, EventArgs e)
        {

            if (PolygonLayerList.SelectedValue != null &&
                SiteList.SelectedValue != null &&
                VariableList.SelectedValue != null &&
                !String.IsNullOrEmpty(OutputSiteName.Text) &&
                !String.IsNullOrEmpty(OutputLayerName.Text))
            {
                sitesPoints.Name = OutputLayerName.Text;
                sitesPoints.Projection = App.Map.Projection;
                sitesPoints.DataTable.Columns.Add(new DataColumn("SiteCode", typeof(string)));

                var symb = new LabelSymbolizer
                {
                    FontColor = Color.Black,
                    FontSize = 10,
                    FontFamily = "Arial",
                    PreventCollisions = true,
                    HaloEnabled = true,
                    HaloColor = Color.White,
                    Orientation = ContentAlignment.MiddleRight,
                    OffsetX = 0.0f,
                    OffsetY = 0.0f,
                };

                AggregateData();

                IFeatureLayer flayer = App.Map.Layers.Add(sitesPoints);

                App.Map.AddLabels(flayer, string.Format("[{0}]", "SiteCode"),
                                    "", symb, "");
                flayer.ShowLabels = true;

                MessageBox.Show("The time series aggregation is completed.", "CRWR Aggregation",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                Parameters_form.ActiveForm.Close();
            }
            else
                MessageBox.Show("Please complete the missing parts of the form.", "CRWR Aggregation");
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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

            //getPolygons((IMapPolygonLayer));
            //getPolygons((IMapPolygonLayer)PolygonLayerList.SelectedValue);
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

            if (variables.Count > 0)
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

        }

        /// <summary>
        /// To be Documented
        /// </summary>
        private void AggregateData()
        {
            foreach (var polygon in polygonData)
            {
                foreach (var site in polygon.sites)
                {
                    if ((String)VariableList.SelectedItem == site.variableName)
                    {
                        site.variableID = getVariableID(site.variableCode);
                        site.siteID = getSiteId(site.siteCode);

                        getSeriesID(site.siteID, site.variableID, polygon);
                    }
                }

                Series seriesToSave = getSeriesFromTable(polygon);
                Theme theme = getThemeParameters();
                // _repositoryManager.SaveSeries(int siteID, int variableID, string methodDescription, string themeName, DataTable dataValues);
                _repositoryManager.SaveSeries(seriesToSave, theme, OverwriteOptions.Append);
            }
        }

        private Series getSeriesFromTable(PolygonData polygon)
        {
            DataTable averageTable = getAverageTable(polygon);
            Series series = new Series();
            series.Site = getSitesParameters(polygon);
            var site = polygon.sites.Find(f => f.variableName == VariableList.SelectedValue.ToString());
            series.Variable = getVariablesParameters(site.variableID);
            series.CreationDateTime = DateTime.Now;
            series.LastCheckedDateTime = DateTime.Now;
            series.UpdateDateTime = DateTime.Now;

            foreach (DataRow row in averageTable.Rows)
            {
                series.AddDataValue((DateTime)row["LocalDateTime"], (Double)row["AVG(DataValues.DataValue)"]);
            }
            return series;
        }

        private Variable getVariablesParameters(int variableID)
        {
            Variable variable = new Variable();
            variable.Code = OutputLayerName.Text + ":" + OutputSiteName.Text;
            variable.Name = VariableList.Text;
            variable.Speciation = "Unknown";
            variable.SampleMedium = "Not Relevant";
            variable.ValueType = "Derived Value";
            variable.IsRegular = false;
            variable.IsCategorical = false;
            //variable.TimeSupport = 0.0;
            variable.DataType = "Average";
            variable.GeneralCategory = "Unknown";
            variable.NoDataValue = -9999;
            int timesUnitsID = getTimeUnitsID(variableID);
            variable.TimeUnit = UnitsRepository.GetByKey(timesUnitsID);
            int variableUnitsID = getVariableUnitsID(variableID);
            variable.VariableUnit = UnitsRepository.GetByKey(variableUnitsID);

            return variable;
        }

        private Site getSitesParameters(PolygonData polygon)
        {
            IFeature centroid = polygon.polygon.Centroid();
            var newpoint = sitesPoints.AddFeature(centroid);

            Site site = new Site();
            var xy = new[] { centroid.Coordinates.First().X, centroid.Coordinates.First().Y };
            String projectionString = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[" +
                "\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            var _wgs84Projection = ProjectionInfo.FromEsriString(projectionString);
            Reproject.ReprojectPoints(xy, new double[] { 0, 0 }, App.Map.Projection, _wgs84Projection, 0, 1);

            int indexNumber = (polygonData.IndexOf(polygon) + 1);

            site.Code = OutputLayerName.Text + ':' + OutputSiteName.Text + indexNumber.ToString();
            newpoint.DataRow.BeginEdit();
            newpoint.DataRow["SiteCode"] = site.Code.ToString();
            newpoint.DataRow.EndEdit();

            site.Name = OutputSiteName.Text + indexNumber.ToString();
            site.Latitude = xy[1];
            site.Longitude = xy[0];
            //site.Elevation_m = 12;
            site.VerticalDatum = "Unkown";
            //site.LocalX = 12;
            //site.LocalY = 12;
            //site.PosAccuracy_m = 12;
            site.State = "";
            site.County = "";
            //site.Comments = "testing";
            //site.Country = "Mexico";
            //site.SiteType = "Type";

            return site;
        }

        private Theme getThemeParameters()
        {
            Theme theme = new Theme();
            theme.Name = "CRWR Aggregation";
            //theme.Description = "";
            return theme;
        }

        private int getVariableID(String variableCode)
        {
            var query =
                "SELECT VariableID FROM Variables WHERE VariableCode = "
                + "'" + variableCode + "'";
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        private int getVariableUnitsID(int variableID)
        {
            var query =
                "SELECT VariableUnitsID FROM Variables WHERE VariableID = "
                + variableID.ToString();
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        private int getTimeUnitsID(int variableID)
        {
            var query =
                "SELECT TimeUnitsID FROM Variables WHERE VariableID = "
                + variableID.ToString();
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
