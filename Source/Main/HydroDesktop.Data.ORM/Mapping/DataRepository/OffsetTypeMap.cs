using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class OffsetTypeMap : ClassMap<OffsetType>
    {
        public OffsetTypeMap()
        {
            Table("OffsetTypes");
            Id(x => x.Id).Column("OffsetTypeID");
            Map(x => x.Description).Column("OffsetDescription").Not.Nullable();

            References(x => x.Unit)
                .Column("OffsetUnitsID")
                .Cascade.SaveUpdate()
                .Not.Nullable().Fetch.Join();
        }
    }
}

