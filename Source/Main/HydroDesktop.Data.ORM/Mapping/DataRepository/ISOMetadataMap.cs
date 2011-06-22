using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class ISOMetadataMap : ClassMap<ISOMetadata>
    {
        public ISOMetadataMap()
        {
            Table("ISOMetadata");
            Id(x => x.Id).Column("MetadataID");
            Map(x => x.TopicCategory).Not.Nullable();
            Map(x => x.Title).Not.Nullable();
            Map(x => x.Abstract).Not.Nullable();
            Map(x => x.ProfileVersion).Not.Nullable();
            Map(x => x.MetadataLink);
        }
    }
}
