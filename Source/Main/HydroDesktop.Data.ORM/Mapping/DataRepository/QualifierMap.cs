using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class QualifierMap : ClassMap<Qualifier>
    {
        public QualifierMap()
        {
            Table("Qualifiers");
            Id(x => x.Id).Column("QualifierID");
            Map(x => x.Code).Column("QualifierCode")
                            .Length(24)
                            .Not.Nullable();

            Map(x => x.Description)
                            .Column("QualifierDescription")
                            .Not.Nullable();
        }
    }
}
