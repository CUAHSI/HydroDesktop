using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using NUnit.Framework;

namespace HydroDesktop.Database.Tests.Database.sqllite
{
    public class SqlIteDataRepositoryTest
    {
        private string goodPath = "sqllitetest.sql3";
        //private string badPath = "";
        private string tempDir = "";
        [SetUp]
        public void setup()
        {
            tempDir =
                Path.Combine(HydroDesktop.Configuration.Settings.Instance.TempDirectory, "sqliteUnitTest");
            Directory.CreateDirectory(tempDir);
        }
        [TearDown]
        public void Teardown()
        {
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CanCreateNewDB()
        {
            string filename = Path.Combine(tempDir, goodPath);
            var dr = SQLiteHelper.CreateSQLiteDatabase(filename);

            Assert.That(File.Exists(filename));

            FileInfo fileInformation = new FileInfo(filename);
            Assert.Greater(fileInformation.Length, 0);
        }

        //[Test]
        //public void FailOnUnwritableCreateNewDB()
        //{
        //    var ac = new DirectorySecurity();
        //    FileSystemAccessRule noWrite = new FileSystemAccessRule();
        //    ac.AddAccessRule();
        //    Directory.SetAccessControl();
        //    var dr = HydroDesktop.Database.sqllite.DataRepository.CreateNewDB(
        //        DataRepository.TemplateDbPath(),  Path.Combine(tempDir,goodPath));
        //}

    }
}
