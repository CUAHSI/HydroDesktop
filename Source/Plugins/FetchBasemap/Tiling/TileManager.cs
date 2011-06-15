using System;
using System.Net;
using System.Drawing;
using System.IO;
using DotSpatial.Topology;
using MWPoint = DotSpatial.Topology.Point;

namespace FetchBasemap.Tiling
{
    class TileManager
    {
        #region Private

        private TileCache _tileCache;
        private string _tileServerUrl;
        private string _tileServerName;

        #endregion



        #region Properties

        public string TileServerURL
        {
            get
            {
                return _tileServerUrl;
            }

        }

        public string TileServerName
        {
            get
            {
                return _tileServerName;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public TileManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileServerName"></param>
        /// <param name="tileServerUrl"></param>
        public TileManager(string tileServerName, string tileServerUrl)
        {
            _tileServerUrl = tileServerUrl;
            _tileServerName = tileServerName;
            
            _tileCache = new TileCache(_tileServerName);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileServerName"></param>
        /// <param name="tileServerUrl"></param>
        public void ChangeService(string tileServerName, string tileServerUrl)
        {
            _tileServerUrl = tileServerUrl;
            _tileServerName = tileServerName;
            
            _tileCache = new TileCache(_tileServerName);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public Tile[,] GetTiles(Envelope envelope, Rectangle bounds)
        {
            Coordinate mapTopLeft = envelope.TopLeft();
            Coordinate mapBottomRight = envelope.BottomRight();

            //Clip the coordinates so they are in the range of the web mercator projection
            mapTopLeft.Y = TileCalculator.Clip(mapTopLeft.Y, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
            mapTopLeft.X = TileCalculator.Clip(mapTopLeft.X, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);

            mapBottomRight.Y = TileCalculator.Clip(mapBottomRight.Y, TileCalculator.MinLatitude, TileCalculator.MaxLatitude);
            mapBottomRight.X = TileCalculator.Clip(mapBottomRight.X, TileCalculator.MinLongitude, TileCalculator.MaxLongitude);

            int zoom = TileCalculator.DetermineZoomLevel(envelope, bounds);

            MWPoint topLeftTileXY = TileCalculator.LatLongToTileXY(mapTopLeft, zoom);

            MWPoint btmRightTileXY = TileCalculator.LatLongToTileXY(mapBottomRight, zoom);

            var tileMatrix = new Tile[(int)(btmRightTileXY.X - topLeftTileXY.X) + 1, (int)(btmRightTileXY.Y - topLeftTileXY.Y) + 1];

            for (var y = (int)topLeftTileXY.Y; y <= btmRightTileXY.Y; y++)
            {
                for (var x = (int)topLeftTileXY.X; x <= btmRightTileXY.X; x++)
                {

                    var currTopLeftPixXY = TileCalculator.TileXYToTopLeftPixelXY(x, y);
                    var currTopLeftCoord = TileCalculator.PixelXYToLatLong((int)currTopLeftPixXY.X, (int)currTopLeftPixXY.Y, zoom);

                    var currBtmRightPixXY = TileCalculator.TileXYToBottomRightPixelXY(x, y);
                    var currBtmRightCoord = TileCalculator.PixelXYToLatLong((int)currBtmRightPixXY.X, (int)currBtmRightPixXY.Y, zoom);

                    var currEnv = new Envelope(currTopLeftCoord, currBtmRightCoord);

                    Tile tile = GetTile(x, y, currEnv, zoom);

                    tileMatrix[x - (int)topLeftTileXY.X, y - (int)topLeftTileXY.Y] = tile;
                }
            }
            
            return tileMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="envelope"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public Tile GetTile(int x, int y, Envelope envelope, int zoom)
        {

            Bitmap bitmap = _tileCache.Get(zoom, x, y);
            if (null != bitmap)
            {
                var tile = new Tile(x, y, zoom, envelope, bitmap);
                return tile;
            }
            try
            {
                string url = _tileServerUrl;

                if (url.Contains("{key}"))
                {
                    string quadKey = TileCalculator.TileXYToBingQuadKey(x, y, zoom);
                    url = url.Replace("{key}", quadKey);

                }
                else
                {
                    url = url.Replace("{zoom}", zoom.ToString());
                    url = url.Replace("{x}", x.ToString());
                    url = url.Replace("{y}", y.ToString());
                }

                   
                var client = new WebClient();
                var stream = client.OpenRead(url);

                if (stream != null) bitmap = new Bitmap(stream);
                
                var tile = new Tile(x, y, zoom, envelope, bitmap);
                
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }


                //Put the tile in the cache
                _tileCache.Put(tile);

                return tile;
            }
            catch (Exception ex)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(ex.Message);

                //Return a No Data Available tile
                var noDataTile = new Tile(x, y, zoom, envelope, Resources.resources.NoDataTile);

                return noDataTile;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="envelope"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public Tile GetTile(MWPoint point, Envelope envelope, int zoom)
        {
            return GetTile((int)point.X, (int)point.Y, envelope, zoom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public Tile GetTileFromLatLong(Coordinate coord, int zoom)
        {
          
            var tileXY = TileCalculator.LatLongToTileXY(coord, zoom);

            //Figure out the extent of the tile so that it can be made into MWImageData
            var tileTopLeftXY = TileCalculator.TileXYToTopLeftPixelXY((int)tileXY.X, (int)tileXY.Y);
            var tileBottomRightXY = TileCalculator.TileXYToTopLeftPixelXY((int)tileXY.X + 1, (int)tileXY.Y + 1);
            
            var tileTopLeft = TileCalculator.PixelXYToLatLong((int)tileTopLeftXY.X, (int)tileTopLeftXY.Y, zoom);
            var tileBottomRight = TileCalculator.PixelXYToLatLong((int)tileBottomRightXY.X, (int)tileBottomRightXY.Y, zoom);

            var envelope = new Envelope(tileTopLeft, tileBottomRight);

            return GetTile(tileXY, envelope, zoom);


        }

    }
}
