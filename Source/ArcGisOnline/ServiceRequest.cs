using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using DotSpatial.Topology;
using System.Data;
using DotSpatial.Projections;
using Newtonsoft.Json.Linq;

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
            string[] fieldNames = new string[] { "Latitude", 
                "Longitude", "SiteName", "SiteCode", "VarCode", "VarName", "ServCode", 
                "watermluri", "StartDate", "EndDate" };
            string queryUri = GenerateQueryUri(url, fieldNames);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    string response = wc.DownloadString(queryUri);
                    int len = response.Length;

                    //Declare Json Elements
                    JObject mainObj = new JObject();
                    JObject outputObj = new JObject();
                    JArray shapeObj = new JArray();

                    mainObj = JObject.Parse(response);
                    shapeObj = mainObj["features"] as JArray;

                    //initialize feature set
                    FeatureSet fs = new FeatureSet(FeatureType.Point);
                    //attr table
                    fs.DataTable.Columns.Add(new DataColumn("Latitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("Longitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("SiteName", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("SiteCode", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("VarCode", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("VarName", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("ServCode", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("WaterMLURI", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("StartDate", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("EndDate", typeof(string)));

                    foreach (JObject feature in shapeObj)
                    {
                        JObject atrList = feature["attributes"] as JObject;

                        double latitude = Convert.ToDouble(atrList["Latitude"]);
                        double longitude = Convert.ToDouble(atrList["Longitude"]);
                        string siteCode = Convert.ToString(atrList["SiteCode"]);
                        string varCode = Convert.ToString(atrList["VarCode"]);
                        string varName = Convert.ToString(atrList["VarName"]);
                        string startDate = Convert.ToString(atrList["StartDate"]);
                        string endDate = Convert.ToString(atrList["EndDate"]);
                        string siteName = Convert.ToString(atrList["SiteName"]);
                        string servCode = Convert.ToString(atrList["ServCode"]);
                        string waterMLURI = Convert.ToString(atrList["WaterMLURI"]);

                        DotSpatial.Topology.Point pt = new DotSpatial.Topology.Point(longitude, latitude);

                        IFeature f = fs.AddFeature(pt);

                        DataRow r = f.DataRow;

                        r["Latitude"] = latitude;
                        r["Longitude"] = longitude;
                        r["SiteCode"] = siteCode;
                        r["VarCode"] = varCode;
                        r["VarName"] = varName;
                        r["ServCode"] = servCode;
                        r["WaterMLURI"] = waterMLURI;
                        r["SiteName"] = siteName;
                        r["StartDate"] = startDate;
                        r["EndDate"] = endDate;
                    }

                    //fs.Projection = KnownCoordinateSystems.Geographic.World.WGS1984);

                    return fs;

                    //outputObj = mainObj["output"] as JsonObject;
                    //shapeObj = outputObj["shape"] as JsonObject;

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
