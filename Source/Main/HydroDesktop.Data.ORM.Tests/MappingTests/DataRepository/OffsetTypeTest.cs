using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database;
using HydroDesktop.Database.Map;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    [TestFixture]
    [Category("Mapping.Data")]
    public class OffsetTypeTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapOffset()
        {
            int id = random.Next();

            Unit offsetUnit = UnitsTest.CreateUnit();
            
            new PersistenceSpecification<OffsetType>(Session)
                .CheckProperty(p => p.Description, "Offset description " + id)
                .CheckReference(p => p.Unit, offsetUnit)
                .VerifyTheMappings();
        }
    }
}