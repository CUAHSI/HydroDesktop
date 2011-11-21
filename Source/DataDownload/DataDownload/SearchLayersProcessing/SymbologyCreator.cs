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

namespace HydroDesktop.DataDownload.SearchLayersProcessing
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
            }
            _imageHelper = new HydroDesktop.WebServices.ServiceIconHelper(_hisCentralUrl);
        }

        readonly string _hisCentralUrl;
        private HydroDesktop.WebServices.ServiceIconHelper _imageHelper;

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
                System.Drawing.Image myImage = _imageHelper.GetImageForService(servCode);
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

