using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Represents a latitude / longitude bounding box.
    /// </summary>
    public class Box
    {
        public Box(double xMin, double xMax, double yMin, double yMax)
        {
            xmin = xMin;
            xmax = xMax;
            ymin = yMin;
            ymax = yMax;
        }
        
        public double xmin;
        public double xmax;
        public double ymin;
        public double ymax;

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", xmin, xmax, ymin, ymax);
        }
    }
}
