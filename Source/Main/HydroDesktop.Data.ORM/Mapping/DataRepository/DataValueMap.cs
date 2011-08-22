using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class DataValueMap : ClassMap<DataValue>
    {
        public DataValueMap()
        {
            Table("DataValues");

            Id(x => x.Id).Column("ValueID");

            Map(x => x.LocalDateTime).Not.Nullable();
            Map(x => x.DateTimeUTC).Not.Nullable();
            Map(x => x.UTCOffset).Not.Nullable();
            Map(x => x.Value).Column("DataValue").Not.Nullable();

            Map(x => x.ValueAccuracy).Not.Nullable();
            Map(x => x.CensorCode).Not.Nullable();
            Map(x => x.OffsetValue);

            References(x => x.Series).Column("SeriesID")
                .Not.Nullable().Cascade.None();

            References(x => x.OffsetType).Column("OffsetTypeID")
                .Nullable().Fetch.Join();
            
            References(x => x.Sample).Column("SampleID")
                .Nullable()
                .Cascade.SaveUpdate().Fetch.Join();
            
            References(x => x.Qualifier).Column("QualifierID")
                .Nullable().Fetch.Join();

            References(x => x.DataFile).Column("FileID")
                .Nullable().Fetch.Join();
        }
    }
}
