using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using System.Net;
using System.Windows.Forms;

namespace HydroDesktop.ArcGisOnline
{
    public class ServiceRequest
    {
        /// <summary>
        /// Given an ArcGIS Online URL, create a DotSpatial in-memory feature set
        /// </summary>
        /// <param name="url">The ArcGIS Online URL</param>
        /// <returns>a feature set that can be added to the map</returns>
        public static IFeatureSet GetFeatures(string url)
        {
            //(1) form the url query
            string[] fieldNames = new string[] { "Latitude", "Longitude", "SiteCode", "VarCode", "VarName", "ServCode", "watermluri" };
            string queryUri = GenerateQueryUri(url, fieldNames);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    string response = wc.DownloadString(queryUri);
                    int len = response.Length;

                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return null;
            }
        }


        //creates the complete query string URI
        private static string GenerateQueryUri(string baseUri, IEnumerable<string> fieldNames)
        {
            var sb = new StringBuilder(baseUri);
            
            if (!(baseUri.EndsWith("/")))
            {
                sb.Append("/");
            }

            sb.Append("0/query?text=&");
            sb.Append("geometry=%7Bxmin%3A+-130%2C+ymin%3A+10%2C+xmax%3A+-60%2C+ymax%3A+60%7D&geometryType=esriGeometryEnvelope&");
            sb.Append("inSR=&spatialRel=esriSpatialRelIntersects&");
            sb.Append("relationParam=&");
            sb.Append("objectIds=&");
            sb.Append("where=&");
            sb.Append("time=&");
            sb.Append("returnCountOnly=false&");
            sb.Append("returnGeometry=false&");
            sb.Append("maxAllowableOffset=&");
            sb.Append("outSR=&");
            sb.Append("outFields=");

            //to append the out fields
            foreach(string fieldName in fieldNames)
            {
                sb.Append(fieldName);
                sb.Append("%2C+");
            }

            string str = sb.ToString();
            if (str.EndsWith("%2C+"))
            {
                str = str.Remove(str.LastIndexOf("%2C+"));
                str = str + "&f=json";
            }

            return str;
            
            
            ///0/query?text=&geometry=%7Bxmin%3A+-130%2C+ymin%3A+10%2C+xmax%3A+-60%2C+ymax%3A+60%7D&geometryType=esriGeometryEnvelope
                //&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&objectIds=&where=&time=&
                //returnCountOnly=false&returnIdsOnly=false&returnGeometry=false&maxAllowableOffset=&outSR=&outFields=Latitude%2C+Longitude%2C+SiteCode%2C+SiteName%2C+VarCode%2C+VarName%2C+ServCode%2C+watermluri&f=html
        }
    }
}
