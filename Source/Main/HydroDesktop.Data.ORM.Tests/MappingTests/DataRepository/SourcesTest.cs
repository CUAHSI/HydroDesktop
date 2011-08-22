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
    [Category("Mapping.Data")]
    public class SourcesTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        
        public void CanMapSource()
        {
            int id = random.Next();

            ISOMetadata metadata = CreateISOMetadata();

            // DataServiceInfo is not used in this model
            //DataServiceInfo serviceInfo = new DataServiceInfo("uri:test","TEST");
            //Session.Save(serviceInfo);

            new PersistenceSpecification<Source>(Session)
                .CheckProperty(p => p.Address, "995 University Blvd")
                .CheckProperty(p => p.Citation, "Kadlec,J.et al.")
                .CheckProperty(p => p.City, "Idaho Falls")
                .CheckProperty(p => p.ContactName, "J. Kadlec")
                .CheckProperty(p => p.Description, "description " + id)
                .CheckProperty(p => p.Email, "mail@gmail.com")
                .CheckProperty(p => p.Link, "www.example.com")
                .CheckProperty(p => p.Organization, "ISU")
                .CheckProperty(p => p.Phone, "2084062978")
                .CheckProperty(p => p.State, "Idaho")
                .CheckProperty(p => p.ZipCode, 83401)

             //   .CheckReference(p=> p.DataService, serviceInfo)

                .CheckReference(p => p.ISOMetadata, metadata)

                .VerifyTheMappings();
        }

        [Test]
        public void CanMapISOMetadata()
        {
            int id = random.Next();

            new PersistenceSpecification<ISOMetadata>(Session)
                .CheckProperty(p => p.Abstract, "abstract " + id)
                .CheckProperty(p => p.MetadataLink, "link " + id)
                .CheckProperty(p => p.ProfileVersion, "profileVersion " + id)
                .CheckProperty(p => p.Title, "title " + id)
                .CheckProperty(p => p.TopicCategory, "category " + id)
                .VerifyTheMappings();
        }

        public static ISOMetadata CreateISOMetadata()
        {
            return ISOMetadata.Unknown;
        }
    }
}