using System;

namespace FacetedSearch3.Settings
{
    public class AreaRectangle
    {
        public AreaRectangle(double xmin, double ymin, double xmax, double ymax)
        {
            XMin = xmin;
            YMin = ymin;
            XMax = xmax;
            YMax = ymax;
        }

        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double YMin { get; private set; }
        public double YMax { get; private set; }

        public override string ToString()
        {
            return string.Format("Point1 (Lng/Lat): {0:N6} {1:N6}" + Environment.NewLine +
                                 "Point2 (Lng/Lat): {2:N6} {3:N6}", XMin, YMin, YMax, YMax);
        }
    }
}
