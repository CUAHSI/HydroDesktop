using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map
{
    public class DataServiceMap : ClassMap<DataServiceInfo>
    {
        public DataServiceMap()
        {
            Table("DataServices");
            Id(x => x.Id).Column("ServiceID");
            Map(x => x.ServiceCode).Column("ServiceCode").Not.Nullable();
            Map(x => x.ServiceName).Not.Nullable();
            Map(x => x.ServiceType).Not.Nullable();
            Map(x => x.Version).Column("ServiceVersion").Not.Nullable();
            Map(x => x.Protocol).Column("ServiceProtocol").Not.Nullable();
            Map(x => x.EndpointURL).Column("ServiceEndpointURL").Not.Nullable();
            Map(x => x.DescriptionURL).Column("ServiceDescriptionURL").Not.Nullable();
            Map(x => x.NorthLatitude);
            Map(x => x.SouthLatitude);
            Map(x => x.EastLongitude);
            Map(x => x.WestLongitude);
            Map(x => x.Abstract);
            Map(x => x.ContactEmail);
            Map(x => x.ContactName);
            Map(x => x.Citation);
            Map(x => x.ServiceTitle);
            
            
            //TODO lab information could be mapped as a component
        }
    }
}
