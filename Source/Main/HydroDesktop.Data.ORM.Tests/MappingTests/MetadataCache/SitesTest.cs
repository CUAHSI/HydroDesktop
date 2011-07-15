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
using HydroDesktop.Database.Map.MetadataCache;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class SitesTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanSaveSite()
        {
            int id = random.Next();
            Site site = CreateSite(id);
            Session.Save(site);
            Session.Flush();

            var fromDB = Session.Get<Site>(site.Id);
            Assert.AreEqual(site, fromDB, "the site before saving and after saving should be equal.");
        }

        public static Site CreateSite(int identifier)
        {
            string prefix = "test";
            double longitude = TestHelpers.RandomLongitude();
            double latitude = TestHelpers.RandomLatitude();
            SpatialReference srsInfo = new SpatialReference(identifier);
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

        public static Site CreateSite(DataServiceInfo service, int identifier)
        {
            Site site = CreateSite(identifier);
            //site.DataService = service;
            return site;
        }
    }
}