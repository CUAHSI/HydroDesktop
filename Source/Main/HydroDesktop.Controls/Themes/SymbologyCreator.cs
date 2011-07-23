using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace HydroDesktop.Controls.Themes
{
    /// <summary>
    /// This class is responsible for creating the "organization logo" symbology
    /// for the search results layer. The image is assigned according
    /// to the "ServiceCode" field value.
    /// </summary>
    public class SymbologyCreator
    {
        public SymbologyCreator(string hisCentralUrl)
        {
            _hisCentralUrl = hisCentralUrl;
            if (_hisCentralUrl.EndsWith("webservices/hiscentral.asmx"))
            {
                _hisCentralUrl = _hisCentralUrl.Substring(0, _hisCentralUrl.IndexOf("webservices/hiscentral.asmx"));
                _defaultIconUrl = _hisCentralUrl + "images/defaulticon.gif";
            }

            LoadIcons();
        }

        readonly string _hisCentralUrl;
        string _defaultIconUrl;

        private readonly Dictionary<string, Image> _serviceIcons = new Dictionary<string, Image>();

        private void LoadIcons()
        {
            ResourceManager rm = Properties.Resources.ResourceManager;

            ResourceSet rs = rm.GetResourceSet(new CultureInfo("en-US"), true, true);

            foreach(DictionaryEntry entry in rs)
            {
                var entryImage = entry.Value as Image;
                if (entryImage != null)
                {
                    _serviceIcons.Add(entry.Key.ToString(), entryImage);
                }
            }
        }

        //public Image GetImageForService(string serviceCode)
        //{
        //    string hdFolder = Path.Combine(Path.GetTempPath(), "hydrodesktop");
        //    if (!Directory.Exists(hdFolder))
        //    {
        //        Directory.CreateDirectory(hdFolder);
        //    }
        //    string imageFolder = Path.Combine(hdFolder, "images");
        //    if (!Directory.Exists(imageFolder))
        //    {
        //        Directory.CreateDirectory(imageFolder);
        //    }

        //    string imageFileName = Path.Combine(imageFolder, serviceCode + ".gif");

        //    try
        //    {
        //        if (!File.Exists(imageFileName))
        //        {
        //            string url = _hisCentralUrl + "getIcon.aspx?name=" + serviceCode;
        //            WebClient client = new WebClient();
        //            client.DownloadFile(url, imageFileName);
        //        }
        //        return Image.FromFile(imageFileName);
        //    }
        //    catch
        //    {
        //        return Properties.Resources.defaulticon;
        //    }
        //}

        public Image GetImageForService(string serviceCode)
        {
            if (_serviceIcons.ContainsKey(serviceCode))
            {
                return _serviceIcons[serviceCode];
            }
            else
            {
                return Properties.Resources.defaulticon;
            }
            
            //try
            //{
            //    string url = _hisCentralUrl + "/getIcon.aspx?name=" + serviceCode;
            //    System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

            //    // Request response:
            //    System.Net.WebResponse webResponse = req.GetResponse();

            //    // Open data stream:
            //    System.IO.Stream webStream = webResponse.GetResponseStream();

            //    // convert webstream to image
            //    System.Drawing.Image tmpImage = System.Drawing.Image.FromStream(webStream);

            //    // Cleanup
            //    webResponse.Close();
            //    return tmpImage;
            //}
            //catch (WebException)
            //{
            //    return Properties.Resources.defaulticon;
            //}
        }
        
        /// <summary>
        /// Create the 'Search Results' layer with image symbology
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public virtual MapPointLayer CreateSearchResultLayer(IFeatureSet fs)
        {
            //get the unique "service code" values
            var serviceCodes = new List<string>();
            foreach (DataRow row in fs.DataTable.Rows)
            {
                string servCode = Convert.ToString(row["ServiceCode"]);
                if (!serviceCodes.Contains(servCode))
                {
                    serviceCodes.Add(servCode);
                }
            }
            serviceCodes.Sort();
            
            var myLayer = new MapPointLayer(fs);
            myLayer.LegendText = "data series";

            var myScheme = new PointScheme();
            myScheme.ClearCategories();

            //assign the categories (could be done with 'editorSettings')
            foreach (string servCode in serviceCodes)
            {
                string filterEx = "[ServiceCode] = '" + servCode + "'";
                System.Drawing.Image myImage = GetImageForService(servCode);
                PointSymbolizer mySymbolizer = new PointSymbolizer(myImage, 14.0);
                PointCategory myCategory = new PointCategory(mySymbolizer);
                myCategory.FilterExpression = filterEx;
                myCategory.LegendText = servCode;
                myCategory.SelectionSymbolizer = new PointSymbolizer(myImage, 16.0);
                myCategory.SelectionSymbolizer.SetFillColor(System.Drawing.Color.Yellow);
                myScheme.AddCategory(myCategory);
            }
            myLayer.Symbology = myScheme;

            return myLayer;
        }
    }
}

