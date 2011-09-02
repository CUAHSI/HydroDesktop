using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using FluentNHibernate.Data;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class SpatialReferenceMap : ClassMap<SpatialReference>
    {
        public SpatialReferenceMap()
        {
            Table("SpatialReferences");
            
            Id(x => x.Id).Column("SpatialReferenceID");

            Map(x => x.SRSID).Column("SRSID").Not.Nullable();
            Map(x => x.SRSName).Not.Nullable();
            Map(x => x.Notes);
        }
    }
}
