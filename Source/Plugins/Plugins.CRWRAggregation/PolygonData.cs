using System.Collections.Generic;
using DotSpatial.Data;

namespace Plugins.CRWRAggregation
{
    class PolygonData
    {
        public IFeature polygon;
        public List<SiteData> sites = new List<SiteData>();
        public List<int> dataSeries = new List<int>();
    }
}
