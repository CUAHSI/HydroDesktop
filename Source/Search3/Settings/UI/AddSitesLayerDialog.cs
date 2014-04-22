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
using Search3.Searching;
using System.Diagnostics;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Data;


namespace Search3.Settings.UI
{
    public partial class AddSitesLayerDialog : Form
    {
        private WaterOneFlowClient waterOneFlowClient;
        private AppManager App;

        public AddSitesLayerDialog(AppManager App)
        {
            this.App = App;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           waterOneFlowClient = new WaterOneFlowClient(urlConnectionTextbox.Text);
    
           // Update service info in the metadata database
           var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;
           var service = waterOneFlowServiceInfo;
           var webService = new WebServiceNode(service.ServiceTitle,
                                  service.ServiceCode, (int) service.Id, service.DescriptionURL, service.EndpointURL,
                                  new Box(service.WestLongitude, service.EastLongitude,
                                          service.SouthLatitude, service.NorthLatitude), service.SiteCount, service.VariableCount, (int)service.ValueCount);
          
           // Get all sites for this service
           IList<Site> siteList;
           siteList = waterOneFlowClient.GetSites();
           var variableList = new List<Tuple<string, string>>();
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
                   var variable = new Tuple<string, string>(series.Variable.Code, series.Variable.Name);
                   variableList.Add(variable);
               }
           }
           foreach (var variable in variableList)
           {
               this.variablesListBox.Items.Add(variable.Item2, false);
               
           }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            waterOneFlowClient = new WaterOneFlowClient(urlConnectionTextbox.Text);
            var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;

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
                        var seriesDataCart = getDataCartFromMetadata(series, waterOneFlowServiceInfo);
                        dataCartSeriesList.Add(seriesDataCart);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                }
                totalDataCartSeriesList.AddRange(dataCartSeriesList);

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
            result.ServCode = WebServiceTitle.Text;

            return result;
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
                Debug.WriteLine("The other part: " + string.Format(Properties.Settings.Default.SearchResultNameMask, key.ServiceCode, timeStamp));
                var fs = key.FeatureSet;

                var filename = Path.Combine(hdProjectPath + "/Search Results",
                                            string.Format(Properties.Settings.Default.SearchResultNameMask, key.ServiceCode, timeStamp));
                fs.Filename = filename;
                fs.Save();
                loadedFeatures.Add(new SearchResultItem(key.ServiceCode, FeatureSet.OpenFile(filename)));
            }
            Debug.WriteLine("Loop done.");

            var _searchSettings = new SearchSettings();
            var searchLayerCreator = new SearchLayerCreator(App.Map, new SearchResult(loadedFeatures), _searchSettings);

            return searchLayerCreator.Create();
        }

       
   
    }
}
