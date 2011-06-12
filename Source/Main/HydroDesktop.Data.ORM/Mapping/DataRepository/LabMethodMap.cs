using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class LabMethodMap : ClassMap<LabMethod>
    {
        public LabMethodMap()
        {
            Table("LabMethods");
            Id(x => x.Id).Column("LabMethodID");
            Map(x => x.LabName).Not.Nullable();
            Map(x => x.LabOrganization).Not.Nullable();
            Map(x => x.LabMethodName).Not.Nullable();
            Map(x => x.LabMethodDescription).Not.Nullable();
            Map(x => x.LabMethodLink);
            
            //TODO lab information could be mapped as a component
        }
    }
}
