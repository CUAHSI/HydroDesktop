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
    public class UnitMap : ClassMap<Unit>
    {
        public UnitMap()
        {
            Table("Units");
            
            Id(x => x.Id).Column("UnitsID");

            Map(x => x.Name).Column("UnitsName")
                            .Length(24)
                            .Not.Nullable();

            Map(x => x.Abbreviation).Column("UnitsAbbreviation");
            Map(x => x.UnitsType).Not.Nullable();
        }
    }
}
