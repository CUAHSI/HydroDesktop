using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class MethodMap : ClassMap<Method>
    {
        public MethodMap()
        {
            Table("MethodsCache");
            Id(x => x.Id).Column("MethodID").GeneratedBy.Increment();
            Map(x => x.Code).Column("OriginMethodId").Not.Nullable();
            Map(x => x.Description).Column("MethodDescription").Not.Nullable();
            Map(x => x.Link).Column("MethodLink").Not.Nullable();

            //References(x => x.DataService).Column("ServiceID").Not.Nullable();
        }
    }
}
