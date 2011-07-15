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
    public class QualifiersTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapQualifiers()
        {
            int id = random.Next();

            new PersistenceSpecification<Qualifier>(Session)
                .CheckProperty(p => p.Code, "QUAL-" + id)
                .CheckProperty(p => p.Description, "this is a sample qualifier.")
                .VerifyTheMappings();
        }

        [Test]
        public void TestCompositeQualifier()
        {
            Qualifier qual1 = CreateQualifier();
            Qualifier qual2 = CreateQualifier();

            Assert.AreNotEqual(qual1, qual2, "the two new qualifiers should not be equal.");

            List<Qualifier> quals = new List<Qualifier>();
            quals.Add(qual1);
            quals.Add(qual2);
            Qualifier composite = Qualifier.CreateCompositeQualifier(quals);

            Session.Save(composite);
            Session.Flush();

            Qualifier composite2 = Session.Get<Qualifier>(composite.Id);

            IList<Qualifier> quals2 = composite2.GetSimpleQualifiers();
            Assert.AreEqual(quals2[0].Code, qual1.Code);
            Assert.AreEqual(quals2[1].Code, qual2.Code);
        }


        public static Qualifier CreateQualifier()
        {
            int rnd = random.Next();
            Qualifier qual = new Qualifier();
            qual.Code = "p" + rnd;
            qual.Description = "provisional data";
            return qual;
        }
    }
}