using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    public class DataServiceMap : ClassMap<DataServiceInfo>
    {
        public DataServiceMap()
        {
            Table("DataServices");
            //Cache.Region("DataService");
            Id(x => x.Id).Column("ServiceID").GeneratedBy.Increment();
            Map(x => x.ServiceCode).Column("ServiceCode").Not.Nullable();
            Map(x => x.ServiceName).Column("ServiceName").Not.Nullable();
            Map(x => x.ServiceType).Not.Nullable();
            Map(x => x.Version).Column("ServiceVersion").Not.Nullable();
            Map(x => x.Protocol).Column("ServiceProtocol").Not.Nullable();
            Map(x => x.EndpointURL).Column("ServiceEndpointURL").Not.Nullable();
            Map(x => x.DescriptionURL).Column("ServiceDescriptionURL").Not.Nullable();
            Map(x => x.NorthLatitude).Not.Nullable();
            Map(x => x.SouthLatitude).Not.Nullable();
            Map(x => x.EastLongitude).Not.Nullable();
            Map(x => x.WestLongitude).Not.Nullable();
            Map(x => x.Abstract).Not.Nullable();
            Map(x => x.ContactEmail).Not.Nullable();
            Map(x => x.ContactName).Not.Nullable();
            Map(x => x.Citation).Not.Nullable();
            Map(x => x.IsHarvested).Not.Nullable();
            Map(x => x.HarveDateTime).Not.Nullable();
            Map(x => x.ServiceTitle);
        }

        public static DataServiceInfo FromDataRow(System.Data.DataRow row)
        {
            DataServiceInfo dsi = new DataServiceInfo();
            dsi.Id = DataReader.ReadInteger(row["ServiceID"]);
            dsi.ServiceCode = DataReader.ReadString(row["ServiceCode"]);
            dsi.ServiceName = DataReader.ReadString(row["ServiceName"]);
            dsi.ServiceType = DataReader.ReadString(row["ServiceType"]);
            dsi.Version = DataReader.ReadDouble(row["ServiceVersion"]);
            dsi.Protocol = DataReader.ReadString(row["ServiceProtocol"]);
            dsi.EndpointURL = DataReader.ReadString(row["ServiceEndpointURL"]);
            dsi.DescriptionURL= DataReader.ReadString(row["ServiceDescriptionURL"]);
            dsi.NorthLatitude= DataReader.ReadDouble(row["NorthLatitude"]);
            dsi.SouthLatitude = DataReader.ReadDouble(row["SouthLatitude"]);
            dsi.EastLongitude = DataReader.ReadDouble(row["EastLongitude"]);
            dsi.WestLongitude = DataReader.ReadDouble(row["WestLongitude"]);
            dsi.Abstract= DataReader.ReadString(row["Abstract"]);
            dsi.ContactEmail = DataReader.ReadString(row["ContactEmail"]);
            dsi.ContactName = DataReader.ReadString(row["ContactName"]);
            dsi.Citation = DataReader.ReadString(row["Citation"]);
            dsi.IsHarvested= DataReader.ReadBoolean(row["IsHarvested"]);
            dsi.HarveDateTime = DataReader.ReadDateTime(row["HarveDateTime"]);
            dsi.ServiceTitle = DataReader.ReadString(row["ServiceTitle"]);
            return dsi;
        }
    }
}
