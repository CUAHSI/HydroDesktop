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

namespace Plugins.CRWRAggregation
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
            PolygonLayerList.SelectedValueChanged += PolySelectionChanged;
            SiteList.SelectedValueChanged += SiteSelectionChanged;
            PolygonLayerList.SelectedIndex = -1;
            SiteList.SelectedIndex = -1;
            VariableList.SelectedIndex = -1;

        }

        /// <summary>
        /// If the polygon layer is selected/changed, the other
        /// variables are cleared and the polygon data is stored.
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
        /// If the site layer is selected/changed, the previous
        /// values are cleared and the variables are (re)populated.
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
        /// Checks that the input string is a valid name for sites in the HydroDesktop SQLite database.
        /// </summary>
        /// <param name="nameToTest"></param>
        /// <returns>Empty string if name is valid.  Otherwise, returns a suggestion for making a valid name.</returns>
        private string resultNameIsValid(string nameToTest)
        {
            // Check for alphanumeric characters only, or underscore, or hyphen
            if (!nameToTest.All(char.IsLetterOrDigit))
            {
                return "Please use only alphanumeric characters, underscores, or hyphens in the name.";
            }
            
            // Make sure this name doesn't already exist in the database
            List<string> siteCodes = getSiteCodes();
            if (siteCodes.Contains("CrwrAggregation:" + nameToTest))
            {
                return "This name already exists in the database.  Please try a different name.";
            }

            // Everything is OK, so return an empty string
            return string.Empty;
        }

        /// <summary>
        /// Run the tool.
        /// </summary>
        private void OK_Click(object sender, EventArgs e)
        {
            if (PolygonLayerList.SelectedValue != null &&
                SiteList.SelectedValue != null &&
                VariableList.SelectedValue != null &&
                !String.IsNullOrEmpty(OutputResultName.Text))
            {
                // Validate site name
                string nameCheckResult = resultNameIsValid(OutputResultName.Text);
                if (nameCheckResult != string.Empty)
                {
                    MessageBox.Show("Invalid site name. " + nameCheckResult, "CRWR Aggregation");
                    return;
                }

                List<string> LegendElements = new List<string>();

                    for (int i = 0; i < PolygonLayerList.Items.Count; i++)
                    {
                        LegendElements.Add(PolygonLayerList.Items[i].ToString().Split(',')[1].Substring(1));
                    }
                    for (int i = 0; i < SiteList.Items.Count; i++)
                    {
                        LegendElements.Add(SiteList.Items[i].ToString().Split(',')[1].Substring(1));
                    }

                    if (!LegendElements.Contains(PolygonLayerList.Text + "_agg]"))
                    {
                        sitesPoints.Name = PolygonLayerList.Text + "_agg";
                    }
                    else
                    {
                        int counter = 1;
                        do
                        {
                            if (!LegendElements.Contains(PolygonLayerList.Text + "_agg (" + counter.ToString("D") + ")]"))
                            {
                                sitesPoints.Name = PolygonLayerList.Text + "_agg (" + counter.ToString("D") + ")";
                            }
                            else
                            {
                                counter = counter + 1;
                            }

                        } while (String.IsNullOrEmpty(sitesPoints.Name));

                    }
                
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
        /// Create a list of the polygon layers available in
        /// the map and populate the dropdown menu.
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
        /// Populate the sites dropdown menu from the
        /// information in the map.
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
        /// Populate the variables available from the selected site
        /// layer, the polygon layer, and the selected polygons in
        /// the map within this layer.
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
        /// This function stores the selected polygons in the
        /// map from the polygon layer.
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
        /// This function retrieves a table from the database,
        /// creates and stores the new time series.
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

        /// <summary>
        /// This function gets the time series (date - value)
        /// </summary>
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

        /// <summary>
        /// This function gets the variable information.
        /// </summary>
        private Variable getVariablesParameters(int variableID)
        {
            Variable variable = new Variable();
            variable.Code = "CrwrAggregation:" + OutputResultName.Text;
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

        /// <summary>
        /// This function gets the site information.
        /// </summary>
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

            site.Code = "CrwrAggregation:" + OutputResultName.Text + indexNumber.ToString();
            newpoint.DataRow.BeginEdit();
            newpoint.DataRow["SiteCode"] = site.Code.ToString();
            newpoint.DataRow.EndEdit();

            site.Name = OutputResultName.Text + indexNumber.ToString();
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

        /// <summary>
        /// This function creates a new theme.
        /// </summary>
        private Theme getThemeParameters()
        {
            Theme theme = new Theme();
            theme.Name = "CRWR Aggregation";
            //theme.Description = "";
            return theme;
        }

        /// <summary>
        /// This function gets the variable's ids.
        /// </summary>
        private int getVariableID(String variableCode)
        {
            var query =
                "SELECT VariableID FROM Variables WHERE VariableCode = "
                + "'" + variableCode + "'";
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// This function gets the variable unit's ids.
        /// </summary>
        private int getVariableUnitsID(int variableID)
        {
            var query =
                "SELECT VariableUnitsID FROM Variables WHERE VariableID = "
                + variableID.ToString();
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// This function gets the time units's ids.
        /// </summary>
        private int getTimeUnitsID(int variableID)
        {
            var query =
                "SELECT TimeUnitsID FROM Variables WHERE VariableID = "
                + variableID.ToString();
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// This function gets the site's ids.
        /// </summary>
        private int getSiteId(String siteCode)
        {
            var query =
                "SELECT SiteID FROM Sites WHERE SiteCode = "
                + "'" + siteCode + "'";
            var result = dbOperations.ExecuteSingleOutput(query);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// This function gets the sitecodes's ids.
        /// </summary>
        private List<string> getSiteCodes()
        {
            var query = "SELECT VariableCode FROM Variables";
            List<string> rowlist = new List<string>();

            DataTable result = dbOperations.LoadTable(query);

            foreach (DataRow row in result.Rows)
                rowlist.Add(row["VariableCode"].ToString());
            return rowlist;
        }

        /// <summary>
        /// This function gets the serie's ids.
        /// </summary>
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

        /// <summary>
        /// This function gets the data values.
        /// </summary>
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
