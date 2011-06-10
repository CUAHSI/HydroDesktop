using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.Database.Map.MetadataCache;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    [TestFixture]
    public class MethodsTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapMethod()
        {
            int id = random.Next();    

            new PersistenceSpecification<Method>(Session)
                .CheckProperty(p => p.Code, id)
                .CheckProperty(p => p.Description, "description1")
                .CheckProperty(p => p.Link, "link1")
                .VerifyTheMappings();
        }

        public static Method CreateMethod(DataServiceInfo service)
        {
            int rnd = random.Next();
            Method m = new Method();
            m.Code = rnd;
            m.Description = "description" + rnd;
            m.Link = "link" + rnd;
            return m;
        }
    }
}