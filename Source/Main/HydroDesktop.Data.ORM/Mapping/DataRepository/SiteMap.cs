using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class SiteMap : ClassMap<Site>
    {
        public SiteMap()
        {
            Table("Sites");
            Id(x => x.Id).Column("SiteID");

            Map(x => x.Code).Column("SiteCode").Not.Nullable();
            Map(x => x.Name).Column("SiteName").Not.Nullable();
            Map(x => x.Latitude).Not.Nullable();
            Map(x => x.Longitude).Not.Nullable();

            References(x => x.SpatialReference).Column("LatLongDatumID").Not.Nullable().Fetch.Join();

            Map(x => x.Elevation_m);
            Map(x => x.LocalX);
            Map(x => x.LocalY);

            References(x => x.LocalProjection).Column("LocalProjectionID").Fetch.Join();

            Map(x => x.PosAccuracy_m);
            Map(x => x.VerticalDatum);
            Map(x => x.State);
            Map(x => x.County);
            Map(x => x.Comments);          
        }
    }
}
