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
    public class QualityControlLevelsTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapQualityControlLevel()
        {
            int id = random.Next();

            new PersistenceSpecification<QualityControlLevel>(Session)
                .CheckProperty(p => p.OriginId, id * 2)
                .CheckProperty(p => p.Code, "code-" + id)
                .CheckProperty(p => p.Definition, "definition " + id)
                .CheckProperty(p => p.Explanation, "explanation " + id)
                
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveQualityControlLevelUnknown()
        {
            QualityControlLevel qcInfo = QualityControlLevel.Unknown;
            qcInfo.OriginId = random.Next();
            Session.Save(qcInfo);
            Session.Flush();

            var fromDB = Session.Get<QualityControlLevel>(qcInfo.Id);
            Assert.AreEqual(qcInfo, fromDB);
        }

        public static QualityControlLevel CreateQualityControlLevel(DataServiceInfo service)
        {
            int id = random.Next();
            QualityControlLevel qcInfo = QualityControlLevel.Unknown;
            qcInfo.OriginId = random.Next();
            qcInfo.Code = "code-" + id;
            qcInfo.Definition = "definition-" + id;
            qcInfo.Explanation = "explanation-" + id;
            //qcInfo.DataService = service;

            return qcInfo;
        }
    }
}