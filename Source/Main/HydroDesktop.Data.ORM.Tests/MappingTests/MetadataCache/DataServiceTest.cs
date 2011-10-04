using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    [TestFixture]
    public class DataServiceTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapDataService()
        {
            int id = random.Next();    

            new PersistenceSpecification<DataServiceInfo>(Session)
                .CheckProperty(p => p.ServiceCode, "servcode-" + id)
                .CheckProperty(p => p.ServiceName, "service name " + id)
                .CheckProperty(p => p.ServiceType, "wateroneflow")
                .CheckProperty(p => p.Version, 1.1)
                .CheckProperty(p => p.Protocol, "soap")
                .CheckProperty(p => p.EndpointURL, "http://" + id)
                .CheckProperty(p => p.DescriptionURL, "http://" + id)
                .CheckProperty(p => p.NorthLatitude, TestHelpers.RandomLatitude())
                .CheckProperty(p => p.SouthLatitude, TestHelpers.RandomLatitude())
                .CheckProperty(p => p.EastLongitude, TestHelpers.RandomLongitude())
                .CheckProperty(p => p.WestLongitude, TestHelpers.RandomLongitude())
                .CheckProperty(p => p.Abstract, "the abstract")
                .CheckProperty(p => p.ContactEmail, "mail@example.com")
                .CheckProperty(p => p.ContactName, "unknown")
                .CheckProperty(p => p.Citation, "unknown")
                .CheckProperty(p => p.IsHarvested, true)
                .CheckProperty(p => p.HarveDateTime, DateTime.Now.Date)
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveDataService()
        {
            DataServiceInfo dsi = CreateDataService();
            Session.Save(dsi);
            Session.Flush();
            var fromDb = Session.Get<DataServiceInfo>(dsi.Id);
            Assert.AreEqual(fromDb, dsi, "the objects should be equal.");
        }

        public static DataServiceInfo CreateDataService()
        {
            int id = random.Next();

            DataServiceInfo dsi = new DataServiceInfo("url:" + id, "title:" + id);
            dsi.ServiceCode = "servcode-" + id;
            dsi.ServiceName = "WaterOneFlow";
            //dsi.Title = "name" + id;
            dsi.ServiceType = "type" + id;
            dsi.Protocol = "protocol" + id;
            dsi.ServiceType = "wateroneflow";
            dsi.Version = 1.0F;
            //dsi.EndpointURL = "url:" + id;
            dsi.DescriptionURL = "url:" + id;
            dsi.NorthLatitude =  (float)TestHelpers.RandomLatitude();
            dsi.SouthLatitude= (float)TestHelpers.RandomLatitude();
            dsi.EastLongitude= (float)TestHelpers.RandomLongitude();
            dsi.WestLongitude= (float)TestHelpers.RandomLongitude();
            dsi.Abstract="the abstract";
            dsi.ContactEmail= "mail@example.com";
            dsi.ContactName= "unknown";
            dsi.Citation = "unknown";
            dsi.IsHarvested = true;
            dsi.HarveDateTime = DateTime.Now.Date;
            return dsi;
        }

    }
}