using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class SeriesMap : ClassMap<Series>
    {
        public SeriesMap()
        {
            Table("DataSeries");
            Id(x => x.Id).Column("SeriesId");
            Map(x => x.IsCategorical).Not.Nullable();
            Map(x => x.BeginDateTime).Not.Nullable();
            Map(x => x.BeginDateTimeUTC).Not.Nullable();
            Map(x => x.EndDateTime).Not.Nullable();
            Map(x => x.EndDateTimeUTC).Not.Nullable();
            Map(x => x.ValueCount).Not.Nullable();
            Map(x => x.CreationDateTime).Not.Nullable();

            Map(x => x.Subscribed);
            Map(x => x.UpdateDateTime);
            Map(x => x.LastCheckedDateTime);

            References(x => x.Site).Column("SiteId").Not.Nullable().Fetch.Join();
            References(x => x.Variable).Column("VariableId").Not.Nullable().Fetch.Join();

            References(x => x.Method).Column("MethodId").Not.Nullable().Fetch.Join();
            References(x => x.QualityControlLevel).Column("QualityControlLevelId").Not.Nullable().Fetch.Join();
            References(x => x.Source).Column("SourceId").Not.Nullable().Fetch.Join();

            HasManyToMany(x => x.ThemeList)
                .Inverse()
                .Table("DataThemes")
                .ParentKeyColumn("SeriesID")
                .ChildKeyColumn("ThemeID")
               .Cascade.AllDeleteOrphan()
                ;

            HasMany(x => x.DataValueList)
                .Inverse()
                .Table("DataValues")
                .KeyColumn("SeriesID").AsList()
                .Cascade.All().BatchSize(100).ExtraLazyLoad();
        }
    }
}
