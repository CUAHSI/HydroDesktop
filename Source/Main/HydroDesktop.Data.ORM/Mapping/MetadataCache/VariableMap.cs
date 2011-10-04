using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class VariableMap : ClassMap<Variable>
    {
        public VariableMap()
        {
            Table("VariablesCache");
            BatchSize(10);
            Id(x => x.Id).Column("VariableID").GeneratedBy.Increment();

            Map(x => x.Code).Column("VariableCode").Length(24).Not.Nullable();
            Map(x => x.Name).Column("VariableName").Not.Nullable();
            Map(x => x.Speciation).Not.Nullable();
            Map(x => x.SampleMedium).Not.Nullable();
            Map(x => x.ValueType).Not.Nullable();
            Map(x => x.DataType).Not.Nullable();
            Map(x => x.GeneralCategory).Not.Nullable();
            Map(x => x.NoDataValue).Not.Nullable();
            Map(x => x.IsRegular).Not.Nullable();
            Map(x => x.TimeSupport).Not.Nullable();

            // the variable units and time units are mapped as 'components'
            Component(x => x.VariableUnit, m =>
            {
                m.Map(x => x.Name).Column("VariableUnitsName").Not.Nullable();
                m.Map(x => x.UnitsType).Column("VariableUnitsType").Not.Nullable();
                m.Map(x => x.Abbreviation).Column("VariableUnitsAbbreviation").Not.Nullable();
            });
            
            Component(x => x.TimeUnit, m =>
            {
                m.Map(x => x.Name).Column("TimeUnitsName").Not.Nullable();
                m.Map(x => x.UnitsType).Column("TimeUnitsType").Not.Nullable();
                m.Map(x => x.Abbreviation).Column("TimeUnitsAbbreviation").Not.Nullable();
            });

            //References(x => x.DataService, "ServiceID").Cascade.SaveUpdate();
        }
    }
}
