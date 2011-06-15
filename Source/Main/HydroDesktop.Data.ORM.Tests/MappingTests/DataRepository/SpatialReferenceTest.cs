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
    public class SpatialReferenceTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapSpatialReference()
        {
            int id = random.Next();

            string srsCode = "EPSG:0001";

            new PersistenceSpecification<SpatialReference>(Session)
                .CheckProperty(p => p.SRSName, "spatial system 1")
                .CheckProperty(p => p.SRSID, id)
                .CheckProperty(p => p.Notes, "notes on " + srsCode)
                .VerifyTheMappings();                
        }

        public static SpatialReference CreateSpatialReference()
        {
            int id = random.Next();
            SpatialReference s = new SpatialReference(id);
            s.SRSID = id;
            s.SRSName = "EPSG:" + id.ToString();
            s.Notes = "notes on " + s.SRSName;
            return s;
        }
    }
}