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
    public class SourcesTest : FixtureBase
    {
        static Random random = new Random();
        static DataServiceInfo service;
        static ISOMetadata metadata;

        [SetUp]
        public void Setup()
        {
            service = DataServiceTest.CreateDataService();
            metadata = CreateISOMetadata();
            
            Session.Save(service);
            Session.Save(metadata);
            Session.Flush();

        }
        [TearDown]
        public void Teardown()
        {
            Session.Delete(metadata);
            Session.Delete(service);
            Session.Flush();

        }

        [Test]
        [Ignore("persistence does not like SetUp attribute")]
        public void CanMapSource()
        {
            int id = random.Next();

          
            
            new PersistenceSpecification<Source>(Session)
                .CheckProperty(p => p.OriginId, id)
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

               // .CheckReference(p => p.DataService, service)
                
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

        [Test]
        public void CanSaveSource()
        {
           
            Source source = Source.Unknown;
          //  source.DataService = service;
            source.ISOMetadata = metadata;
            //Session.Save(service);
            Session.Save(source);
            Session.Flush();

            var fromdb = Session.Get<Source>(source.Id);
            Assert.IsNotNull(fromdb);
            Assert.AreEqual(source, fromdb);

            Session.Delete(source);
        }

        public static ISOMetadata CreateISOMetadata()
        {
            ISOMetadata iso = ISOMetadata.Unknown;
          
            return iso;
        }

        public static Source CreateSource(DataServiceInfo service, ISOMetadata metadata1)
        {
            int rnd = random.Next();
            Source src = new Source();
            src.OriginId = rnd;
            src.Address = "995 University Blvd";
            src.Citation = "Kadlec,J.et al.";
            src.City = "Idaho Falls";
            src.ContactName = "J. Kadlec";
            src.Description = "description " + rnd;
            src.Email = "mail@gmail.com";
            src.Link = "www.example.com";
            src.Organization = "ISU";
            src.Phone = "2084062978";
            src.State = "Idaho";
            src.ZipCode = 83401;
          //  src.DataService = service;
            src.ISOMetadata = metadata1;
            return src;
        }
    }
}