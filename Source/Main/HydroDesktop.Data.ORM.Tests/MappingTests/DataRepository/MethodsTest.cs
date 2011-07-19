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
    public class MethodsTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapMethod()
        {
            int id = random.Next();

            new PersistenceSpecification<Method>(Session)
                .CheckProperty(p => p.Description, "SaveTheme Description " + id)
                .CheckProperty(p => p.Link, "SaveTheme Link " + id)
                
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveMethodUnknown()
        {
            Method Method = Method.Unknown;
            Session.Save(Method);
            Session.Flush();

            var fromDB = Session.Get<Method>(Method.Id);
            Assert.AreEqual(Method, fromDB);
        }
    }
}