using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class ThemeMap : ClassMap<Theme>
    {     
        public ThemeMap()
        {
            Table("DataThemeDescriptions");
            Id(x => x.Id).Column("ThemeID");

            Map(x => x.Description).Column("ThemeDescription").Nullable();
            Map(x => x.Name).Column("ThemeName").Not.Nullable();
            Map(x => x.DateCreated).Not.Nullable();

            HasManyToMany(x => x.SeriesList)
                .Table("DataThemes")
                .ParentKeyColumn("ThemeID")
                .ChildKeyColumn("SeriesID")
                //.Inverse()
                .BatchSize(10)
                .Cascade.AllDeleteOrphan().Fetch.Join()
                ;
        }
    }
}
