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
    public class SamplesTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapLabMethod()
        {
            int id = random.Next();

            new PersistenceSpecification<LabMethod>(Session)
                .CheckProperty(p => p.LabName, "laboratory " + id)
                .CheckProperty(p => p.LabOrganization, "ISU water quality lab")
                .CheckProperty(p => p.LabMethodName, "lab metod " + id)
                .CheckProperty(p => p.LabMethodDescription, "this is a lab method described in " + id)
                .CheckProperty(p => p.LabMethodLink, "unknown")
                .VerifyTheMappings();
        }

        [Test]
        public void CanMapSample()
        {
            int id = random.Next();

            LabMethod labMethod = CreateLabMethod();

            new PersistenceSpecification<Sample>(Session)
                .CheckProperty(p => p.LabSampleCode, "code-" + id)
                .CheckProperty(p => p.SampleType, "sample type " + id)
                .CheckReference(p => p.LabMethod, labMethod)
                .VerifyTheMappings();
        }

        public void CanSaveSamples()
        {
            Sample sample1 = CreateSample();
            Sample sample2 = CreateSample();
            sample2.LabMethod = sample1.LabMethod;

            Session.Save(sample1);
            Session.Save(sample2);
            Session.Flush();

            Sample sample11 = Session.Get<Sample>(sample1.Id);
            Sample sample22 = Session.Get<Sample>(sample2.Id);

            Assert.AreNotEqual(sample11, sample22, "the two samples should be different.");
            Assert.AreEqual(sample11.LabMethod, sample22.LabMethod, 
                            "the two samples should have the same lab method.");
        }

        

        public static LabMethod CreateLabMethod()
        {
            int rnd = random.Next(100);
            LabMethod labMethod = new LabMethod();
            labMethod.LabName = "lab" + rnd;
            labMethod.LabOrganization = "ISU lab" + rnd;
            labMethod.LabMethodName = "ISU lab metod" + rnd;
            labMethod.LabMethodDescription = "this method is described in manual " + rnd;
            labMethod.LabMethodLink = "unknown link";
            return labMethod;
        }

        public static Sample CreateSample()
        {
            int rnd = random.Next(200);
            Sample sample = new Sample();
            sample.LabSampleCode = "code-" + rnd;
            sample.SampleType = "sample type 1";
            sample.LabMethod = CreateLabMethod();
            return sample;
        }
    }
}