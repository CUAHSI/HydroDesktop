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
    public class DataFileTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapDataFile()
        {
            int id = random.Next();
            QueryInfo query = QueryInfoTest.CreateQueryInfo();

            new PersistenceSpecification<DataFile>(Session)
                .CheckProperty(p => p.FileName, "values.xml")
                .CheckProperty(p => p.FileType, "xml")
                .CheckProperty(p => p.FileDescription, "xml file returned by WaterOneFlow GetValues service.")
                .CheckProperty(p => p.FilePath, System.IO.Path.GetTempPath())
                .CheckProperty(p => p.FileOrigin, "web service")
                .CheckProperty(p => p.LoadMethod, "search plug-in")
                .CheckProperty(p => p.LoadDateTime, TestHelpers.CurrentDateTime())
                .CheckReference(p => p.QueryInfo, query)
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveDataFiles()
        {
            DataFile file1 = CreateDataFileInfo();
            DataFile file2 = CreateDataFileInfo();
            DataFile file3 = CreateDataFileInfo();

            file1.QueryInfo.DataService = file2.QueryInfo.DataService;
            file3.QueryInfo = null;
            
            Session.Save(file1);
            Session.Save(file2);
            Session.Save(file3);
            Session.Flush();

            DataFile f1 = Session.Get<DataFile>(file1.Id);
            DataFile f2 = Session.Get<DataFile>(file2.Id);
            DataFile f3 = Session.Get<DataFile>(file3.Id);

            Assert.AreEqual(f1.QueryInfo.DataService, f2.QueryInfo.DataService, "the two data files should come from the  same data service");

            Assert.IsTrue(f2.IsFromWaterOneFlow, "file2 should be associated with a data service");
            Assert.IsFalse(f3.IsFromWaterOneFlow, "file3 should not have a data service");
        }

        public static DataFile CreateDataFileInfo()
        {
            int rnd = random.Next(200);
            DataFile file = new DataFile();
            file.FileName = "values" + rnd + ".xml";
            file.FileType = "xml";
            file.FilePath = System.IO.Path.GetTempPath();
            file.FileOrigin = "web service";
            file.FileDescription = "WaterML file";
            file.LoadMethod = "download from web service";
            file.LoadDateTime = TestHelpers.CurrentDateTime();
            file.QueryInfo = QueryInfoTest.CreateQueryInfo();
            
            return file;
        }
    }
}