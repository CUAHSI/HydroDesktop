using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class QueryInfoMap : ClassMap<QueryInfo>
    {
        public QueryInfoMap()
        {
            Table("Queries");

            Id(x => x.Id).Column("QueryID");
            Map(x => x.LocationParameter).Not.Nullable();
            Map(x => x.VariableParameter).Not.Nullable();
            Map(x => x.BeginDateParameter).Not.Nullable();
            Map(x => x.EndDateParameter).Not.Nullable();
            Map(x => x.AuthenticationToken).Not.Nullable();
            Map(x => x.QueryDateTime).Not.Nullable();

            References(x => x.DataService).Column("ServiceID")
                                          .Not.Nullable()
                                          .Cascade.SaveUpdate();
        }
    }
}
