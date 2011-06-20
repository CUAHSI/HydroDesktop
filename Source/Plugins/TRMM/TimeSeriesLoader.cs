using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using DotSpatial.Data;
using System.Data;
using System.IO;
using DotSpatial.Topology;
using DotSpatial.Projections;

namespace trmm
{
    public class TimeSeriesLoader
    {
        public static DataTable GetTimeSeries(double lat, double lon, DateTime start, DateTime end)
        {
            //form the parameters
            double west = 0.1 * Math.Floor(lon * 10);
            double east = 0.1 * Math.Ceiling(lon * 10);
            double north = 0.1 * Math.Ceiling(lat * 10);
            double south = 0.1 * Math.Floor(lat * 10);

            double byr = start.Year;
            double bmo = start.Month;
            double bdy = start.Day;

            double eyr = end.Year;
            double emo = end.Month;
            double edy = end.Day;
            
            //form the URL
            string url = String.Format(@"http://disc2.nascom.nasa.gov/daac-bin/Giovanni/tovas/Giovanni_cgi.pl?" +
            @"west={0}&north={1}&east={2}&south={3}&params=0|3B42_V6&" +
            @"plot_type=Time+Plot&byr={4}&bmo={5}&bdy={6}&eyr={7}&emo={8}&edy={9}&" +
            @"begin_date=1998%2F01%2F01&end_date=2010%2F10%2F31&" +
            @"cbar=cdyn&cmin=&cmax=&yaxis=ydyn&ymin=&ymax=&yint=&ascres=0.25x0.25&" +
            @"global_cfg=tovas.global.cfg.pl&instance_id=TRMM_V6&prod_id=3B42_daily&action=ASCII+Output",
            west, north, east, south, byr, bmo, bdy, eyr, emo, edy);

            //read the ASCII file
            DataTable tab = new DataTable();
            tab.Columns.Add(new DataColumn("time", typeof(DateTime)));
            tab.Columns.Add(new DataColumn("pcp", typeof(double)));

            WebClient webCli = new WebClient();
            string stri = webCli.DownloadString(url);

            StringReader rdr = new StringReader(stri);
            string line = rdr.ReadLine();
            bool saving = false;
            while (!String.IsNullOrEmpty(line))
            {
                line = rdr.ReadLine();
                if (saving == false)
                {
                    if (line.StartsWith("Time"))
                        saving = true;
                }
                else if (!String.IsNullOrEmpty(line))
                {
                    string[] vals = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length == 2)
                    {
                        string timestr = vals[0];
                        string[] timefields = timestr.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                        DateTime time = new DateTime(Convert.ToInt32(timefields[0]), Convert.ToInt32(timefields[1]), Convert.ToInt32(timefields[2]));
                        string pcp = vals[1];
                        DataRow row = tab.NewRow();
                        row[0] = time;
                        row[1] = pcp;
                        tab.Rows.Add(row);
                    }
                }
            }
            rdr.Close();

            return tab;
        }

        public static DataTable GetGrid(double lonMin, double lonMax, double latMin, double latMax, DateTime start, DateTime end)
        {
            //form the parameters
            double west = Math.Round(lonMin, 3);
            double east = Math.Round(lonMax, 3);
            double north = Math.Round(latMax, 3);
            double south = Math.Round(latMin, 3);

            double byr = start.Year;
            double bmo = start.Month;
            double bdy = start.Day;

            double eyr = end.Year;
            double emo = end.Month;
            double edy = end.Day;

            //form the URL
            string url = String.Format(@"http://disc2.nascom.nasa.gov/daac-bin/Giovanni/tovas/Giovanni_cgi.pl?" +
            @"west={0}&north={1}&east={2}&south={3}&params=0|3B42_V6&" +
            @"plot_type=Area+Plot&byr={4}&bmo={5}&bdy={6}&eyr={7}&emo={8}&edy={9}&" +
            @"begin_date=1998%2F01%2F01&end_date=2010%2F10%2F31&" +
            @"cbar=cdyn&cmin=&cmax=&yaxis=ydyn&ymin=&ymax=&yint=&ascres=0.25x0.25&" +
            @"global_cfg=tovas.global.cfg.pl&instance_id=TRMM_V6&prod_id=3B42_daily&action=ASCII+Output",
            west, north, east, south, byr, bmo, bdy, eyr, emo, edy);

            //read the ASCII file
            DataTable tab = new DataTable();
            tab.Columns.Add(new DataColumn("lat", typeof(double)));
            tab.Columns.Add(new DataColumn("lon", typeof(double)));
            tab.Columns.Add(new DataColumn("pcp", typeof(double)));

            WebClient webCli = new WebClient();
            string stri = webCli.DownloadString(url);

            StringReader rdr = new StringReader(stri);
            string line = rdr.ReadLine();
            bool saving = false;
            while (!String.IsNullOrEmpty(line))
            {
                line = rdr.ReadLine();
                if (saving == false)
                {
                    if (line.StartsWith("Latitude"))
                        saving = true;
                }
                else if (!String.IsNullOrEmpty(line))
                {
                    string[] vals = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length == 3)
                    {
                        string lat = vals[0];
                        string lon = vals[1];
                        string pcp = vals[2];
                        DataRow row = tab.NewRow();
                        row[0] = lat;
                        row[1] = lon;
                        row[2] = pcp;
                        tab.Rows.Add(row);
                    }
                }
            }
            rdr.Close();

            return tab;

        }

        public static FeatureSet MakeFeatureSet(DataTable table)
        {
            FeatureSet fs = new FeatureSet(FeatureType.Point);
            DataTable tab = fs.DataTable;
            tab.Columns.Add(new DataColumn("lat", typeof(double)));
            tab.Columns.Add(new DataColumn("lon", typeof(double)));
            tab.Columns.Add(new DataColumn("pcp", typeof(double)));

            foreach (DataRow row in table.Rows)
            {
                double lat = Convert.ToDouble(row[0]);
                double lon = Convert.ToDouble(row[1]);
                double pcp = Convert.ToDouble(row[2]);
                Coordinate cGeog = new Coordinate(lon, lat);
                Coordinate cMerc = LonLat2WebMerc(cGeog);
                Feature f = new Feature(FeatureType.Point, new Coordinate[] { cMerc });

                fs.Features.Add(f);

                DataRow r = f.DataRow;
                r[0] = lat;
                r[1] = lon;
                r[2] = pcp;
            }
            return fs;
        }

        static Coordinate LonLat2WebMerc(Coordinate lonLat)
        {
           
            double[] xy = new double[2];
            xy[0] = lonLat.X;
            xy[1] = lonLat.Y;

            double[] z = new double[1];
            //Try to project here
            Reproject.ReprojectPoints(xy, z, KnownCoordinateSystems.Geographic.World.WGS1984, KnownCoordinateSystems.Projected.World.WebMercator, 0, 1);

            Coordinate result = new Coordinate();
            result.X = xy[0];
            result.Y = xy[1];

            return result;
        }
    }
}
