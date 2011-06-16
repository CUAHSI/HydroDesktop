using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
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
                .CheckProperty(p => p.Abstract, "995 University Blvd")
                .CheckProperty(p => p.Citation, "Kadlec,J.et al.")
                .CheckProperty(p => p.DescriptionURL, "Idaho Falls")
                .CheckProperty(p => p.ContactName, "J. Kadlec")
                .CheckProperty(p => p.EndpointURL, "description " + id)
                .CheckProperty(p => p.ContactEmail, "mail@gmail.com")
              //  .CheckProperty(p => p.IsHarvested, harvest) // not mapped
                .CheckProperty(p => p.ServiceCode, "ISU")
                .CheckProperty(p => p.ServiceName, "2084062978")
                .CheckProperty(p => p.ServiceType, "Idaho")
                .CheckProperty(p => p.Version, 8340.0)


                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveDataService()
        {
            int id = random.Next();

            DataServiceInfo serviceInfo = new DataServiceInfo("uri:test", "TEST");
            
            Session.Save(serviceInfo);

            var fromDb1 = Session.Get <DataServiceInfo>(serviceInfo.Id);

            Assert.AreEqual(fromDb1.ServiceTitle, serviceInfo.ServiceTitle);
            Assert.AreEqual(fromDb1.EndpointURL, serviceInfo.EndpointURL);

        }
    }
}
