using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class SiteMap : ClassMap<Site>
    {
        public SiteMap()
        {
            Table("SitesCache");
          //  BatchSize(100).Cache.Region("Sites");
            Id(x => x.Id).Column("SiteID").GeneratedBy.Increment();

            Map(x => x.Code).Column("SiteCode").Not.Nullable();
            Map(x => x.Name).Column("SiteName").Not.Nullable();
            Map(x => x.Latitude).Not.Nullable();
            Map(x => x.Longitude).Not.Nullable();

            Component(x => x.SpatialReference, m =>
            {
                m.Map(x => x.SRSID).Column("LatLongDatumSRSID").Not.Nullable();
                m.Map(x => x.SRSName).Column("LatLongDatumName").Not.Nullable();
            });

            Map(x => x.Elevation_m);
            Map(x => x.LocalX);
            Map(x => x.LocalY);

            Component(x => x.LocalProjection, m =>
            {
                m.Map(x => x.SRSID).Column("LocalProjectionSRSID");
                m.Map(x => x.SRSName).Column("LocalProjectionName");
            });

            Map(x => x.PosAccuracy_m);
            Map(x => x.State);
            Map(x => x.County);
            Map(x => x.Comments);

            //References(x => x.DataService).Column("ServiceID").Cascade.SaveUpdate;
        }
    }
}
