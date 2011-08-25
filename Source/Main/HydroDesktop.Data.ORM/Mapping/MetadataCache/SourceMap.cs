using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class SourceMap : ClassMap<Source>
    {
        public SourceMap()
        {
            Table("SourcesCache");
            Id(x => x.Id).Column("SourceID").GeneratedBy.Increment();
            Map(x => x.OriginId).Column("OriginSourceID").Not.Nullable();
            Map(x => x.Organization).Column("Organization").Not.Nullable();
            Map(x => x.Description).Column("SourceDescription").Not.Nullable();
            Map(x => x.Link).Column("SourceLink").Not.Nullable();
            Map(x => x.ContactName).Not.Nullable();
            Map(x => x.Phone).Not.Nullable();
            Map(x => x.Email).Not.Nullable();
            Map(x => x.Address).Not.Nullable();
            Map(x => x.City).Not.Nullable();
            Map(x => x.State).Not.Nullable();
            Map(x => x.ZipCode).Not.Nullable();
            Map(x => x.Citation).Not.Nullable();

            References(x => x.ISOMetadata).Column("MetadataID").Nullable().Fetch.Join();
          //  References(x => x.DataService).Column("ServiceID").Not.Nullable().Fetch.Join();
        }
    }
}
