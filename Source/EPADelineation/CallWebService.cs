using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using Newtonsoft.Json.Linq;

namespace EPADelineation
{
    class CallWebService
    {
        # region Variables

        private Coordinate _stPoint;

        private string _PointIndexingUrl = "http://iaspub.epa.gov/WATERSWebServices/OWServices";
        
        private string _DelineationUrl = "http://iaspub.epa.gov/waters10/waters_services.navigationDelineationService";

        private string _StreamlineUrl = "http://iaspub.epa.gov/waters10/waters_services.upstreamDownStreamService";

        private ProjectionInfo _defaultProjection;

        #endregion

        # region Constructor

        public CallWebService(Coordinate stPoint)
        {
            //This is a three dimensional coordinate with z=0
            _stPoint = stPoint;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the Start Point for Delineation/Upstream
        /// </summary>
        /// <returns>Returns an object[] of the start point</returns>
        public object[] GetStartPoint()
        {
            string[] startpt = new string[2];

            startpt = GetPointInputREST();

            return startpt;
        }

        /// <summary>
        /// Public Methods to get delineated watershed
        /// </summary>
        /// <returns>Returns an object[] including the delineated watershed polygon</returns>
        public object[] GetWsheds(object[] startPoint)
        {
            if (startPoint == null) return null;
            if (startPoint.Length < 2) return null;
            
            string Comid = startPoint[0] as string;
            string Measure = startPoint[1] as string;

            //Get Delineated Watersheds
            string wshedUri = GetWshedQueryUri(Comid, Measure);
            object[] wshedObj = GetDelineation(wshedUri);

            if (wshedObj != null)
            {
                IFeatureSet DelineatePoly = wshedObj[0] as IFeatureSet;
                string area = wshedObj[1] as string;

                return wshedObj;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Public Methods to Get Upstream Flowlines
        /// </summary>
        /// <param name="startPoint">Get startPoint</param>
        /// <returns>Returns an object[] including the streamlines information</returns>
        public object[] GetLines(object[] startPoint)
        {
            if (startPoint == null) return null;
            if (startPoint.Length < 2) return null;
            
            string Comid = startPoint[0] as string;
            string Measure = startPoint[1] as string;

            //Get upstream flowlines
            string streamUri = GetStreamQueryUri(Comid, Measure);
            object[] streamObj = GetStreamline(streamUri);

            if (streamObj != null)
            {   
                IFeatureSet Upstream = streamObj[0] as IFeatureSet;
                List<string> comid = streamObj[1] as List<string>;
                List<string> reachcode = streamObj[2] as List<string>;
                List<string> totdist = streamObj[3] as List<string>;

                return streamObj;
            }
            else
            {
                return null;
            }

        }

        #endregion 
        
        #region Private Methods

        /// <summary>
        /// Gets inputs for point indexing service and call the service methods.
        /// This uses the HTTP GET version of the request
        /// </summary>
        /// <returns>Returns Start Comid and Measure</returns>
        private string[] GetPointInputREST()
        {
            int comid = 0;
            double measure = 0;
            
            //create the input parameters
            string pointUri = GetPointQueryUri(_stPoint);

            //Declare a WebClient instance to get the Watershed Delineation response string
            WebClient delineate = new WebClient();

            try
            {
                string response = delineate.DownloadString(pointUri);
                using (XmlTextReader reader = new XmlTextReader(new StringReader(response)))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "comid")
                        {
                            string comidStr = reader.ReadInnerXml();
                            comid = Convert.ToInt32(comidStr, CultureInfo.InvariantCulture);
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "fmeasure")
                        {
                            string measureStr = reader.ReadInnerXml();
                            measure = Convert.ToDouble(measureStr, CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error calling Delineation web service. " + ex.Message);
                return null;
            }

            if (comid > 0 && measure > 0)
            {

                string[] startpt = new string[2];
                startpt[0] = comid.ToString(CultureInfo.InvariantCulture);
                startpt[1] = measure.ToString(CultureInfo.InvariantCulture);

                return startpt;
            }
            else
            {
                MessageBox.Show("No point returned. Please select a different point.");
                return null;
            }
        }

        /// <summary>
        /// Gets the point query Uri
        /// </summary>
        /// <param name="stPoint">the point (longitude, latitude coordinates)</param>
        /// <returns>the uri</returns>
        private string GetPointQueryUri(Coordinate stPoint)
        {
            // The Max Distance is set as 100km. Non-limited distance could cause timeout.
            string uri = String.Format("{0}?invoke=pointIndexingServiceSimple&pInputGeometry=POINT({1}+{2})" +
                "&pInputGeometrySrid=8265&pReachresolution=3&pPointIndexingMethod=Distance" +
                "&pPointIndexingFcodeAllow=&pPointIndexingFcodeDeny=&pPointIndexingMaxDist=100" +
                "&pPointIndexingRaindropDist=100&pOutputPathFlag=&pTolerance=5",
                _PointIndexingUrl, stPoint.X.ToString(CultureInfo.InvariantCulture), stPoint.Y.ToString(CultureInfo.InvariantCulture));
            return uri;
        }

        /// <summary>
        /// Get the query string for EPA Delineation HTTP requirement
        /// </summary>
        /// <param name="qcomid">COMID from the PointIndexing Service</param>
        /// <param name="qmeasure">Measure from the PointIndexing Service</param>
        /// <returns>Returns the query url</returns>
        private string GetWshedQueryUri(string qcomid, string qmeasure)
        {
            // The Max Distance is set as 100km. Non-limited distance could cause timeout.
            string uri = _DelineationUrl + "?pNavigationType=UT&pStartComid=" + qcomid + "&pStartMeasure=" + qmeasure + 
                "&pMaxDistance=100&pMaxTime=&pAggregationFlag=true&pOutputFlag=FEATURE&pFeatureType=CATCHMENT_TOPO" +
                "&optCache=1269303461090&optOutGeomFormat=GEOJSON&optJSONPCallback=success";
            return uri;
        }

        /// <summary>
        /// Get the query string for EPA Upstrem/Downstream HTTP requirement
        /// </summary>
        /// <param name="qcomid">COMID from the PointIndexing Service</param>
        /// <param name="qmeasure">Measure from the PointIndexing Service</param>
        /// <returns>Returns the query url</returns>
        private string GetStreamQueryUri(string qcomid, string qmeasure)
        {
            // The Max Distance is set as 100km. Non-limited distance could cause timeout.
            string uri = _StreamlineUrl + "?pNavigationType=UT&pStartComid=" + qcomid + "&pStartMeasure=" + qmeasure +
                "&pStopDistancekm=100&pStopTimeOfTravel=&pFlowlinelist=true&pTraversalSummary=true" +
                "&optCache=1269303461090&optOutGeomFormat=GEOJSON&optJSONPCallback=success";

            return uri;
        }

        /// <summary>
        /// Get delineated watershed polygon from EPA WebServices
        /// </summary>
        /// <param name="uri">Query string</param>
        /// <returns>Returns an IFeatureSet including the delineated polygon</returns>
        private object[] GetDelineation(string uri)
        {
            //Declare a WebClient instance to get the Watershed Delineation response string
            WebClient delineate = new WebClient();

            try
            {
                string response = delineate.DownloadString(uri);

                int start = response.IndexOf("(");
                int end = response.IndexOf(")");

                response = response.Substring(start + 1, end - 1 - start);

                //Declare Json Elements
                JObject mainObj = new JObject();
                JToken outputObj = new JObject();
                JToken shapeObj = new JObject();

                mainObj = JObject.Parse(response);

                outputObj = mainObj["output"];
                shapeObj = outputObj["shape"];

                string stype = shapeObj["type"].ToString();
                string area = outputObj["total_areasqkm"].ToString();

                JArray coordArray = shapeObj["coordinates"] as JArray;

                //For coordinate information
                string lat;
                string lon;

                //Setup projection information
                _defaultProjection = KnownCoordinateSystems.Projected.World.WebMercator;
                ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

                //Initialize feature parameters
                Feature polyf = new Feature();
                Feature multipolyf = new Feature();
                IFeatureSet polyfs = new FeatureSet(FeatureType.Polygon);

                //For the case GeoJSON returns a MultiPolygon 
                if (stype.Trim().ToLower() == "multipolygon")
                {
                    foreach (JArray Polycoord in coordArray) //The third level branket
                    {
                        Polygon[] polys = new Polygon[Polycoord.Count]; ;

                        if (Polycoord != null)
                        {
                            for (int i = 0; i < Polycoord.Count; i++)//The second level branket
                            {
                                JArray multiRingcoord = (JArray)Polycoord[i];

                                IList<Coordinate> multicoords = new List<Coordinate>();

                                if (multiRingcoord != null)
                                {
                                    foreach (JArray latlongcoord in multiRingcoord) //The first level branket
                                    {
                                        Coordinate coord = new Coordinate();

                                        lon = latlongcoord[0].ToString();
                                        lat = latlongcoord[1].ToString();

                                        coord.X = Convert.ToDouble(lon);
                                        coord.Y = Convert.ToDouble(lat);

                                        double[] xy = new double[2];
                                        xy[0] = coord.X;
                                        xy[1] = coord.Y;

                                        double[] z = new double[1];

                                        coord.X = xy[0];
                                        coord.Y = xy[1];

                                        multicoords.Add(coord);
                                    }

                                    polys[i] = new Polygon(multicoords);
                                }
                            }

                            //Save polygon[] into a multipolygon
                            if (polys.Length > 1)
                            {
                                IMultiPolygon multipolys = new MultiPolygon(polys);

                                multipolyf = new Feature(multipolys);
                            }
                            else
                            {
                                // special case: A multipolygon with one part
                                multipolyf = new Feature(polys[0]);
                            }

                            //Save features into a featureset
                            if (polyfs.Features.Count == 0)
                            {
                                polyfs.Projection = _defaultProjection;
                                polyfs = new FeatureSet(multipolyf.FeatureType);
                                polyfs.AddFeature(multipolyf);
                            }

                            else
                            {
                                polyfs.AddFeature(multipolyf);
                            }
                        }
                    }
                }

                //For the case GeoJSON returns a Polygon 
                if (stype.Trim().ToLower() == "polygon")
                {
                    foreach (JArray Ringcoord in coordArray)  //The second level branket
                    {
                        IList<Coordinate> coords = new List<Coordinate>();

                        if (Ringcoord != null)
                        {

                            foreach (JArray latlongcoord in Ringcoord) //The first level branket
                            {
                                Coordinate coord = new Coordinate();

                                lon = latlongcoord[0].ToString();
                                lat = latlongcoord[1].ToString();

                                coord.X = Convert.ToDouble(lon);
                                coord.Y = Convert.ToDouble(lat);

                                double[] xy = new double[2];
                                xy[0] = coord.X;
                                xy[1] = coord.Y;

                                double[] z = new double[1];

                                coord.X = xy[0];
                                coord.Y = xy[1];

                                coords.Add(coord);
                            }

                            polyfs.Projection = _defaultProjection;
                            polyf = new Feature(FeatureType.Polygon, coords);
                        }

                        polyfs = new FeatureSet(polyf.FeatureType);

                        if (polyfs.Features.Count == 0)
                        {
                            polyfs = new FeatureSet(polyf.FeatureType);
                            polyfs.AddFeature(polyf);
                        }

                        else
                        {
                            polyfs.AddFeature(polyf);
                        }
                    }
                }

                object[] watersheds = new object[2];

                watersheds[0] = polyfs as object;
                watersheds[1] = area as object;

                return watersheds;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Watershed not found. Please try a different point.");
                return null;
            }

            catch (Exception ex2)
            {
                MessageBox.Show("Error searching for watershed. " + ex2.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Upstream flowslines from EPA WebServices
        /// </summary>
        /// <param name="uri">Query string</param>
        /// <returns>Returns an IFeatureSet including the </returns>
        private object[] GetStreamline(string uri)
        {
            //Declare a WebClient instance to get the Watershed Delineation response string
            WebClient streamline = new WebClient();

            try
            {
                string response = streamline.DownloadString(uri);

                int start = response.IndexOf("(");
                int end = response.IndexOf(")");

                response = response.Substring(start + 1, end - 1 - start);

                //Declare Json Elements
                JObject mainObj = new JObject();
                JToken outputObj;
                JArray lineObj = new JArray();
                JToken shapeObj = new JObject();

                mainObj = JObject.Parse(response);

                outputObj = mainObj["output"];
                lineObj = outputObj["flowlines_traversed"] as JArray;

                List<string> comid = new List<string>();
                List<string> reachcode = new List<string>();
                List<string> totdist = new List<string>();

                //Setup projection information
                _defaultProjection = KnownCoordinateSystems.Projected.World.WebMercator;
                ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

                //Initialize feature parameters
                Feature linef = new Feature();
                IFeatureSet linefs = new FeatureSet(FeatureType.Line);

                //for (int i = 0; i < lineObj.Count; i++)
                foreach (JObject flowObj in lineObj)
                {
                    shapeObj = flowObj["shape"] as JObject;

                    string id = flowObj["comid"].ToString();
                    string code = flowObj["reachcode"].ToString();
                    string dist = flowObj["totaldist"].ToString();

                    string stype = shapeObj["type"].ToString();

                    JArray coordArray = shapeObj["coordinates"] as JArray;

                    //For coordinate information
                    string lat;
                    string lon;

                    //For the case GeoJSON returns a MultiLineString 
                    if (stype.Trim().ToLower() == "multilinestring")
                    {
                        if (coordArray != null)
                        {
                            LineString[] lines = new LineString[coordArray.Count];

                            for (int j = 0; j < coordArray.Count; j++)//The second level branket
                            {
                                JArray linecoord = (JArray)coordArray[j];

                                IList<Coordinate> multicoords = new List<Coordinate>();

                                if (linecoord != null)
                                {
                                    foreach (JArray latlongcoord in linecoord) //The first level branket
                                    {
                                        Coordinate coord = new Coordinate();

                                        lon = latlongcoord[0].ToString();
                                        lat = latlongcoord[1].ToString();

                                        coord.X = Convert.ToDouble(lon);
                                        coord.Y = Convert.ToDouble(lat);

                                        multicoords.Add(coord);
                                    }

                                    lines[j] = new LineString(multicoords);
                                }
                            }

                            //Save lines[] into a multiline
                            IMultiLineString multilines = new MultiLineString(lines);

                            linef = new Feature(multilines);
                        }
                    }

                    //For the case GeoJSON returns a LineString 
                    if (stype.Trim().ToLower() == "linestring")
                    {
                        IList<Coordinate> coords = new List<Coordinate>();
                        foreach (JArray latlongcoord in coordArray)  //The second level branket
                        {
                            Coordinate coord = new Coordinate();

                            lon = latlongcoord[0].ToString();
                            lat = latlongcoord[1].ToString();

                            coord.X = Convert.ToDouble(lon, CultureInfo.InvariantCulture);
                            coord.Y = Convert.ToDouble(lat, CultureInfo.InvariantCulture);

                            coords.Add(coord);
                        }

                        linef = new Feature(FeatureType.Line, coords);
                    }

                    linefs.Projection = _defaultProjection;

                    //Save features into a featureset
                    if (linefs.Features.Count == 0)
                    {
                        linefs = new FeatureSet(linef.FeatureType);
                        linefs.AddFeature(linef);
                    }

                    else
                    {
                        linefs.AddFeature(linef);
                    }

                    //Save streamlines' information
                    comid.Add(id);
                    reachcode.Add(code);
                    totdist.Add(dist);
                }

                object[] streamlines = new object[4];

                streamlines[0] = linefs as object;
                streamlines[1] = comid as object;
                streamlines[2] = reachcode as object;
                streamlines[3] = totdist as object;

                return streamlines;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        #endregion
    }
}
