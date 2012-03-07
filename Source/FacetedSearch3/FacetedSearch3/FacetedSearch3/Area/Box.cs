using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacetedSearch3.Area
{

    // Represents a latitude / longitude bounding box.
    public class Box
    {
        public Box(double xMin, double xMax, double yMin, double yMax) 
        { 
            XMin = xMin;
            XMax = xMax;
            YMax = yMax;
            YMin = yMin;     
        }

        public double XMax { get; set;}

        public double XMin { get; set; }

        public double YMax { get; set; }

        public double YMin { get; set; }

        //public override string ToString();
    }
}
