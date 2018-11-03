using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces.ObjectModel;
using System.Net;
using System.IO;
using HydroDesktop.WebServices;
using System.Globalization;
using HydroDesktop.Plugins.Search.Searching;
using System.Diagnostics;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Data;
using HydroDesktop.Plugins.Search.Settings;
using HydroDesktop.Plugins.Search;


namespace Search3.Settings.UI
{
    public partial class AddSitesLayerDialog : Form
    {
        private WaterOneFlowClient waterOneFlowClient;
        private AppManager App;
        private List<string> checkedVariables = new List<string>();

        public AddSitesLayerDialog(AppManager App)
        {
            this.App = App;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            variablesListBox.Items.Clear();

            waterOneFlowClient = new WaterOneFlowClient(urlTextbox.Text);

            // Update service info in the metadata database
            var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;
            var service = waterOneFlowServiceInfo;
           
            // Get all sites for this service
            IList<Site> siteList;
            siteList = waterOneFlowClient.GetSites();
            var variableList = new List<string>();
            foreach (var site in siteList)
            {
                // Get series for this site
                IList<SeriesMetadata> currentSeriesList;
                try
                {
                    currentSeriesList = waterOneFlowClient.GetSiteInfo(site.Code);
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        var rdr = new StreamReader(ex.Response.GetResponseStream());
                        rdr.ReadToEnd();
                    }

                    continue;
                }
                // Save series info to metadata cache database
                foreach (var series in currentSeriesList)
                {
                    if (!variableList.Contains(series.Variable.Name))
                    {
                        var variable = series.Variable.Name;
                        variableList.Add(variable);
                    }
                }
            }
            foreach (var variable in variableList)
            {
                this.variablesListBox.Items.Add(variable, false);
            }
            if (variablesListBox.Items.Count != 0)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            variablesListBox.Enabled = false;
            urlTextbox.Enabled = false;
            titleTextbox.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            waterOneFlowClient = new WaterOneFlowClient(urlTextbox.Text);
            var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;

            // Trim the query off of the URL if it still exists
            int index = waterOneFlowServiceInfo.EndpointURL.IndexOf("?");
            if (index > -1)
            {
                waterOneFlowServiceInfo.EndpointURL = waterOneFlowServiceInfo.EndpointURL.Substring(0, index);
            }

            var serviceInfo = new DataServiceInfo();
            serviceInfo.IsHarvested = false;
            serviceInfo.ServiceName = waterOneFlowServiceInfo.ServiceName;
            serviceInfo.Version = waterOneFlowServiceInfo.Version;
            serviceInfo.ServiceType = waterOneFlowServiceInfo.ServiceType;
            serviceInfo.Protocol = waterOneFlowServiceInfo.Protocol;
            serviceInfo.VariableCount = waterOneFlowServiceInfo.VariableCount;

            IList<Site> siteList;
            siteList = waterOneFlowClient.GetSites();

            // Default extent for the service.  These values are designed to be overwritten as we query sites in the service
            double east = -180;
            double west = 360;
            double north = -90;
            double south = 90;
            int valueCount = 0;
            var totalDataCartSeriesList = new List<SeriesDataCart>();

            foreach (var site in siteList)
            {
                // Get series for this site
                IList<SeriesMetadata> currentSeriesList;
                var dataCartSeriesList = new List<SeriesDataCart>();
                try
                {
                    currentSeriesList = waterOneFlowClient.GetSiteInfo(site.Code);
                }
                catch (WebException ex)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    continue;
                }

                // Update service extent 
                if (site.Latitude > north)
                {
                    north = site.Latitude;
                }
                if (site.Latitude < south)
                {
                    south = site.Latitude;
                }
                if (site.Longitude > east)
                {
                    east = site.Longitude;
                }
                if (site.Longitude < west)
                {
                    west = site.Longitude;
                }

                // Save series info to metadata cache database
                foreach (var series in currentSeriesList)
                {
                    valueCount += series.ValueCount;
                    try
                    {
                        if (checkedVariables.Count != 0)
                        {
                            if (checkedVariables.Contains(series.Variable.Name))
                            {
                                var seriesDataCart = getDataCartFromMetadata(series, waterOneFlowServiceInfo);
                                dataCartSeriesList.Add(seriesDataCart);
                            }
                        }
                        else
                        {
                            var seriesDataCart = getDataCartFromMetadata(series, waterOneFlowServiceInfo);
                            dataCartSeriesList.Add(seriesDataCart);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                }
                totalDataCartSeriesList.AddRange(dataCartSeriesList);
                variablesListBox.Enabled = true;
                urlTextbox.Enabled = true;
                titleTextbox.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }

            // Update service info
            serviceInfo.IsHarvested = true;
            serviceInfo.HarveDateTime = DateTime.Now;
            serviceInfo.EastLongitude = east;
            serviceInfo.WestLongitude = west;
            serviceInfo.NorthLatitude = north;
            serviceInfo.SouthLatitude = south;

            serviceInfo.SiteCount = siteList.Count;
            serviceInfo.ValueCount = valueCount;
            serviceInfo.VariableCount = waterOneFlowServiceInfo.VariableCount;

            SearchResult resultFeatureSet = null;
            if (totalDataCartSeriesList.Count > 0)
            {
                resultFeatureSet = SearchHelper.ToFeatureSetsByDataSource(totalDataCartSeriesList);
            }

            if (resultFeatureSet != null)
            {
                //We need to reproject the Search results from WGS84 to the projection of the map.
                var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
                foreach (var item in resultFeatureSet.ResultItems)
                    item.FeatureSet.Projection = wgs84;
                var layers = ShowSearchResults(resultFeatureSet);
                Debug.WriteLine("ShowSearchResults done.");

                // Unselect all layers in legend (http://hydrodesktop.codeplex.com/workitem/8559)
                App.Map.MapFrame.GetAllLayers().ForEach(r => r.IsSelected = false);

                // Select first search result layer

                var first = layers.FirstOrDefault().GetParentItem();
                if (first != null)
                {
                    first.IsSelected = true;
                }

                // Unselect "Map Layers" legend item (http://hydrodesktop.codeplex.com/workitem/8458)
                App.Legend.RootNodes
                    .ForEach(delegate(ILegendItem item)
                    {
                        if (item.LegendText == "Map Layers")
                        {
                            item.IsSelected = false;
                        }
                    });
                Debug.WriteLine("Finished.");
            }
        }

        private SeriesDataCart getDataCartFromMetadata(SeriesMetadata series, DataServiceInfo waterOneFlowServiceInfo)
        {
            var result = new SeriesDataCart();
            Site cSite = series.Site;
            result.SiteName = cSite.Name;
            result.SiteCode = cSite.Code;
            result.Latitude = cSite.Latitude;
            result.Longitude = cSite.Longitude;

            Variable v = series.Variable;
            result.VariableName = v.Name;
            result.VariableCode = v.Code;
            result.DataType = v.DataType;
            result.ValueType = v.ValueType;

            result.SampleMedium = v.SampleMedium;
            result.TimeSupport = Convert.ToDouble(v.TimeSupport, CultureInfo.InvariantCulture);
            result.GeneralCategory = v.GeneralCategory;
            result.TimeUnit = v.TimeUnit.Name;

            result.BeginDate = Convert.ToDateTime(series.BeginDateTime, CultureInfo.InvariantCulture);
            result.EndDate = Convert.ToDateTime(series.EndDateTime, CultureInfo.InvariantCulture);
            result.ValueCount = series.ValueCount;

            result.ServURL = waterOneFlowServiceInfo.EndpointURL;
            result.ServCode = titleTextbox.Text;

            return result;
        }

        private void CheckFields()
        {
            if (errorProvider1.GetError(titleTextbox).Equals("")
                && errorProvider1.GetError(urlTextbox).Equals("")
                && !String.IsNullOrEmpty(titleTextbox.Text)
                && !String.IsNullOrEmpty(urlTextbox.Text))
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }



        /// <summary>
        /// Displays search results (all data series and sites complying to the search criteria)
        /// </summary>
        public IEnumerable<IMapPointLayer> ShowSearchResults(SearchResult searchResult)
        {
            //try to save the search result layer and re-add it
            var hdProjectPath = HydroDesktop.Configuration.Settings.Instance.CurrentProjectDirectory;
            String timeStamp = string.Format("{0}_{1}{2}{3}",
                DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            var loadedFeatures = new List<SearchResultItem>(searchResult.ResultItems.Count());
            Debug.WriteLine(searchResult.ResultItems.Count());
            foreach (var key in searchResult.ResultItems)
            {
                Debug.WriteLine("hdProjectPath: " + hdProjectPath + "/Search Results");
                Debug.WriteLine("The other part: " + string.Format(HydroDesktop.Plugins.Search.Properties.Settings.Default.SearchResultNameMask, key.ServiceCode, timeStamp));
                var fs = key.FeatureSet;

                var filename = Path.Combine(hdProjectPath + "/Search Results",
                                            string.Format(HydroDesktop.Plugins.Search.Properties.Settings.Default.SearchResultNameMask, key.ServiceCode, timeStamp));
                fs.Filename = filename;
                fs.Save();
                loadedFeatures.Add(new SearchResultItem(key.ServiceCode, FeatureSet.OpenFile(filename)));
            }
            Debug.WriteLine("Loop done.");

            var _searchSettings = new SearchSettings();
            var searchLayerCreator = new SearchLayerCreator(App.Map, new SearchResult(loadedFeatures), _searchSettings);

            return searchLayerCreator.Create();
        }

        private void titleTextbox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(titleTextbox, !String.IsNullOrEmpty(titleTextbox.Text) ? "" : "Please enter a title");
            CheckFields();
        }

        private void urlTextbox_Validated(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(urlTextbox.Text))
            {
                errorProvider1.SetError(urlTextbox, "Please enter a valid URL");
                CheckFields();
                return;
            }

            if (!urlTextbox.Text.StartsWith("http://") && !urlTextbox.Text.StartsWith("https://"))
            {
                urlTextbox.Text = "http://" + urlTextbox.Text; //add http:// to the beginning
            }
            //check that the URL is Valid -- this is quite slow. Perhaps a REGEX would be better.
            errorProvider1.SetError(urlTextbox, !WebOperations.IsUrlFormatValid(urlTextbox.Text) ? "Please enter a valid URL" : "");

            CheckFields();
        }

        private void variablesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                checkedVariables.Add(variablesListBox.Items[e.Index] as string);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                checkedVariables.Remove(variablesListBox.Items[e.Index] as string);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < variablesListBox.Items.Count; i++)
            {
                variablesListBox.SetItemChecked(i, true);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < variablesListBox.Items.Count; i++)
            {
                variablesListBox.SetItemChecked(i, false);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            variablesListBox.Items.Clear();
            checkedVariables.Clear();
            urlTextbox.Clear();
            titleTextbox.Clear();
            urlTextbox.Clear();
            titleTextbox.Enabled = true;
            urlTextbox.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

        }
    }

    class WebOperations
    {
        #region Public Members
        /// <summary>
        /// Determines if the format of a URL string is valid
        /// </summary>
        /// <param name="url">The URL string to check</param>
        /// <returns></returns>
        public static bool IsUrlFormatValid(string url)
        {
            try
            {
                Uri urlCheck = new Uri(url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
