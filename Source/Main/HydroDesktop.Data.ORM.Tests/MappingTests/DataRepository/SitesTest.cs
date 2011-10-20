using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    [Category("Mapping.Data")]
    public class SitesTest : FixtureBase
    {
        static Random random = new Random();
        public SitesTest()
        {
           
        }

        [Test]
        public void CanCorrectlyMapSite()
        {
            int id = random.Next();
            
            SpatialReference srs1 = SpatialReferenceTest.CreateSpatialReference();
            SpatialReference srs2 = SpatialReferenceTest.CreateSpatialReference();
            
            new PersistenceSpecification<Site>(Session)
                .CheckReference(p => p.SpatialReference, srs1)
                .CheckReference(p => p.LocalProjection, srs2)
                .CheckProperty(p => p.Code, "NWISDV:" + id)
                .CheckProperty(p => p.Name, "Site-" + id)
                .CheckProperty(p => p.Longitude, -120.0)
                .CheckProperty(p => p.Latitude, 50.0)
                .CheckProperty(p => p.Elevation_m, 789.0)
                .CheckProperty(p => p.LocalX, 1987346.0)
                .CheckProperty(p => p.LocalY, 239876.0)
                .CheckProperty(p => p.PosAccuracy_m, 2.0)
                .CheckProperty(p => p.State, "Idaho")
                .CheckProperty(p => p.County, "Bonneville")
                .CheckProperty(p => p.Comments, "No Comment")
                .VerifyTheMappings();
        }

        //[Test]
        //public void CanCorrectlyMapSite2()
        //{
        //    int identifer = random.Next();
        //    DataSourceCode Network = new DataSourceCode
        //    {
        //        PrefixCode = "TEST2"
        //    };
        //    // with one property
        //    LocationGeographicPoint point = CreateNewPoint();
        //    List<PropertyGeneric> properties = new List<PropertyGeneric>(1);
        //    properties.Add(PropertiesTests.CreateNewProperty(identifer));
        //    new PersistenceSpecification<Site>(Session)
        //         .CheckReference(p => p.Network, Network)
        //             .CheckProperty(p => p.Name, "Site-1002")
        //              .CheckProperty(p => p.Code, "1002")
        //              .CheckReference(p => p.GeographicLocation, point)
        //              .CheckList(p => p.SiteProperties, properties)
        //              .VerifyTheMappings();
        //}
        //[Test]
        //public void CanCorrectlyMapSite3()
        //{
        //    int identifer = random.Next(); 
        //    DataSourceCode Network = new DataSourceCode
        //    {
        //        PrefixCode = "TEST3"
        //    }; // with one property
        //    LocationGeographicPoint point = CreateNewPoint();
        //    List<PropertyGeneric> properties = new List<PropertyGeneric>(2);
        //    properties.Add(PropertiesTests.CreateNewProperty(identifer));

        //    properties.Add(PropertiesTests.CreateNewProperty(identifer+1));

        //    new PersistenceSpecification<Site>(Session)
        //        .CheckReference(p => p.Network, Network)
        //        .CheckProperty(p => p.Name, "Site-1002")
        //        .CheckProperty(p => p.Code, "1002")
        //        .CheckReference(p => p.GeographicLocation, point)
        //        .CheckList(p => p.SiteProperties, properties)
        //        .VerifyTheMappings();
        //}

        [Test]
        public void CanCorrectlyMapManySites()
        {
            int identifier = random.Next();

            SpatialReference datum = SpatialReferenceTest.CreateSpatialReference();
            Session.Save(datum);

            List<Site> sites = new List<Site>();

            var site1 = CreateSite(identifier);
            site1.SpatialReference = datum;           
            sites.Add(site1);

            var site2 = CreateSite(identifier);
            site2.Name = "D";
            site2.Code = "E";
            site2.SpatialReference = datum;
            sites.Add(site2);

            foreach (var site in sites)
            {
                Session.Save(site);
            }
            Session.Flush();

            //var fromDb = Session.Get<Site>(site1.Id);
            //Assert.AreEqual(2, fromDb.SiteProperties.Count,
            //    "Site 1 properties count prior to add should be 2");

            //fromDb = Session.Get<Site>(site2.Id);
            //Assert.AreEqual(3, fromDb.SiteProperties.Count,
            //    "Site 2 properties count prior to add should be 2");

            //var stateProperty2 = new PropertyString
            //                         {
            //                             Name = "State",
            //                             StringValue = "CA",
            //                             Uri = "uri:AdlFeatureThesarus:AdminLevel2"
            //                         };

            //site1.SiteProperties.Add(stateProperty2);
            //Session.SaveOrUpdate(site1);
            //Session.Flush();

            //fromDb = Session.Get<Site>(site1.Id);
            //Assert.AreEqual(3, fromDb.SiteProperties.Count,
            //    "Site 1 properties count prior to add should be 3");

            //fromDb = Session.Get<Site>(site2.Id);
            //Assert.AreEqual(3, fromDb.SiteProperties.Count,
            //    "Site 2 properties count prior to add should still be 3");

        }

        public static Site CreateSite(int identifier)
        {
            string prefix = "test";
            double longitude = random.NextDouble() * 360 - 180;
            double latitude = random.NextDouble() * 180 - 90;
            SpatialReference srsInfo = SpatialReferenceTest.CreateSpatialReference();
            string siteName = "Site " + identifier.ToString();
            string siteCode = prefix + ":" + identifier.ToString();

            return new Site
                       {
                           Name = siteName,
                           Code = siteCode,
                           Longitude = longitude,
                           Latitude = latitude,
                           SpatialReference = srsInfo,
                           Comments = "Testing Site.."
                       };
        }
    }
}