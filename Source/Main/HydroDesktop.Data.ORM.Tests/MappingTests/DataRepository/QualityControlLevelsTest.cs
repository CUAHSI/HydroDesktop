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
    public class QualityControlLevelsTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapQualityControlLevel()
        {
            int id = random.Next();

            new PersistenceSpecification<QualityControlLevel>(Session)
                .CheckProperty(p => p.Code, "code-" + id)
                .CheckProperty(p => p.Definition, "definition " + id)
                .CheckProperty(p => p.Explanation, "explanation " + id)
                
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveQualityControlLevelUnknown()
        {
            QualityControlLevel qcInfo = QualityControlLevel.Unknown;
            Session.Save(qcInfo);
            Session.Flush();

            var fromDB = Session.Get<QualityControlLevel>(qcInfo.Id);
            Assert.AreEqual(qcInfo, fromDB);
        }
    }
}