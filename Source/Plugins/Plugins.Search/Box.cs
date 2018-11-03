using System;

namespace HydroDesktop.Plugins.Search
{
    /// <summary>
    /// Represents a latitude / longitude bounding box.
    /// </summary>
    public class Box
    {
        /// <summary>
        /// Bounding Box for HydroDesktop Search
        /// </summary>
        /// <param name="xMin">minimum x longitude</param>
        /// <param name="xMax">maximum x longitude</param>
        /// <param name="yMin">minimum y latitude</param>
        /// <param name="yMax">maximum y latitude</param>
        public Box(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }
        /// <summary>
        /// Minimum X (latitude)
        /// </summary>
        public double XMin { get; private set; }
        /// <summary>
        /// Maximum X (latitude)
        /// </summary>
        public double XMax { get; private set; }
        /// <summary>
        /// Minimum Y (longitude)
        /// </summary>
        public double YMin { get; private set; }
        /// <summary>
        /// Maximum Y (longitude)
        /// </summary>
        public double YMax { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Point1 (Lng/Lat): {0:N6} {1:N6} " + Environment.NewLine +
                                 "Point2 (Lng/Lat): {2:N6} {3:N6} ", XMin, YMin, YMax, YMax);
        }
    }
}
