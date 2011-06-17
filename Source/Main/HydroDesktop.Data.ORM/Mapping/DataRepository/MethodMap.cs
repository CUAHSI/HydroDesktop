using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class MethodMap : ClassMap<Method>
    {
        public MethodMap()
        {
            Table("Methods");
            Id(x => x.Id).Column("MethodID");
            Map(x => x.Description).Column("MethodDescription");
            Map(x => x.Link).Column("MethodLink");
        }
    }
}
