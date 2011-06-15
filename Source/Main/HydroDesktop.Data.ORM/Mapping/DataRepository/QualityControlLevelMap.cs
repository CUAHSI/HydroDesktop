using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class QualityControlLevelMap : ClassMap<QualityControlLevel>
    {
        public QualityControlLevelMap()
        {
            Table("QualityControlLevels");
            Id(x => x.Id).Column("QualityControlLevelID");
            Map(x => x.Code).Column("QualityControlLevelCode").Not.Nullable();

            Map(x => x.Definition).Not.Nullable();
            Map(x => x.Explanation).Not.Nullable();
        }
    }
}
