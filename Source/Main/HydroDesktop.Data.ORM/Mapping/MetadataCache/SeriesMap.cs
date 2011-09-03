using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class SeriesMap : ClassMap<SeriesCache>
    {
        public SeriesMap()
        {
            Table("DataSeriesCache");
            //BatchSize(1000).Cache.Region("Series");
            Id(x => x.Id).Column("SeriesId").GeneratedBy.Increment();
            Map(x => x.BeginDateTime).Not.Nullable();
            Map(x => x.BeginDateTimeUTC).Not.Nullable();
            Map(x => x.EndDateTime).Not.Nullable();
            Map(x => x.EndDateTimeUTC).Not.Nullable();
            Map(x => x.ValueCount);

            References(x => x.Site).Column("SiteID").Not.Nullable().Fetch.Join();
            References(x => x.Variable).Column("VariableID").Not.Nullable().Fetch.Join();

            References(x => x.Method).Column("MethodID").Not.Nullable().Fetch.Join();
            References(x => x.QualityControlLevel).Column("QualityControlLevelID").Not.Nullable().Fetch.Join();
            References(x => x.Source).Column("SourceID").Not.Nullable().Fetch.Join();

            References(x => x.DataService).Column("ServiceID").Not.Nullable().Fetch.Join();          
        }
    }
}
