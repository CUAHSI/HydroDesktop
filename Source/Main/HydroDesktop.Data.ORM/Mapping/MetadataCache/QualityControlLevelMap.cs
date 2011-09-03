using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class QualityControlLevelMap : ClassMap<QualityControlLevel>
    {
        public QualityControlLevelMap()
        {
            Table("QualityControlLevelsCache");
            Id(x => x.Id).Column("QualityControlLevelID").GeneratedBy.Increment();
            Map(x => x.Code).Column("QualityControlLevelCode").Not.Nullable();
            Map(x => x.OriginId).Column("OriginQualityControlLevelID").Not.Nullable();
            Map(x => x.Definition).Not.Nullable();
            Map(x => x.Explanation).Not.Nullable();

            //References(x => x.DataService).Column("ServiceID").Cascade.SaveUpdate();
        }
    }
}
