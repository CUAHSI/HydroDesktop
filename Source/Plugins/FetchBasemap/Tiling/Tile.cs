using System.Drawing;
using MWPoint = DotSpatial.Topology.Point;
using DotSpatial.Topology;

namespace FetchBasemap.Tiling
{
    class Tile
    {
        public Tile(int x, int y, int zoomLevel, Envelope envelope, Bitmap bitmap)
        {
            X = x;
            Y = y;
            ZoomLevel = zoomLevel;
            Envelope = envelope;
            Bitmap = bitmap;
        }

        #region Properties

        public int X { get; set; }

        public int Y { get; set; }

        public int ZoomLevel { get; set; }

        public Envelope Envelope { get; set; }

        public Bitmap Bitmap { get; set; }

        public MWPoint TileXY
        {
            get { return new MWPoint(X, Y); }
        }
        
        #endregion

    }
}
