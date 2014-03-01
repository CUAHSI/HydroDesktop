using System.Collections.Generic;
using DotSpatial.Data;

namespace Aggregation_Plugin
{
    class PolygonData
    {
        public IFeature polygon;
        public List<SiteData> sites = new List<SiteData>();
        public List<int> dataSeries = new List<int>();
    }
}
