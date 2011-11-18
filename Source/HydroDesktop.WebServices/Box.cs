using System;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Represents a latitude / longitude bounding box.
    /// </summary>
    public class Box
    {
        public Box(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }

        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double YMin { get; private set; }
        public double YMax { get; private set; }

        public override string ToString()
        {
            return string.Format("Point1 (Lng/Lat): {0:N6} {1:N6} " + Environment.NewLine +
                                 "Point2 (Lng/Lat): {2:N6} {3:N6} ", XMin, YMin, YMax, YMax);
        }
    }
}
