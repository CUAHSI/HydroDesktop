using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class VariableMap : ClassMap<Variable>
    {
        public VariableMap()
        {
            Table("Variables");
            Id(x => x.Id).Column("VariableID");

            Map(x => x.Code).Column("VariableCode").Length(24).Not.Nullable();
            Map(x => x.Name).Column("VariableName").Not.Nullable();
            Map(x => x.Speciation).Not.Nullable();
            Map(x => x.GeneralCategory).Not.Nullable();
            Map(x => x.SampleMedium).Not.Nullable();
            Map(x => x.ValueType).Not.Nullable();
            Map(x => x.IsRegular).Not.Nullable();
            Map(x => x.IsCategorical).Not.Nullable();
            Map(x => x.DataType).Not.Nullable();
            Map(x => x.TimeSupport).Not.Nullable();
            Map(x => x.NoDataValue).Not.Nullable();

            References(x => x.VariableUnit, "VariableUnitsID").Not.Nullable().Fetch.Join();
            References(x => x.TimeUnit, "TimeUnitsID").Not.Nullable().Fetch.Join();
        }
    }
}
