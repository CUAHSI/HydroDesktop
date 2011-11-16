using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using NUnit.Framework;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Interfaces.Tests
{
    [TestFixture()]
    public class HydroDatabaseTest
    {
        /*
        private string cleanDb1;
        private string cleanDb2;

        [SetUp]
        public void SetupNewDatabase()
        {
            SetupSQLLite();

            string tempPath = Path.GetTempFileName();
            tempPath = Path.ChangeExtension(tempPath, "sqllite");
            cleanDb1 = CreateNewDB(tempPath);

            tempPath = Path.GetTempFileName();
            tempPath = Path.ChangeExtension(tempPath, "sqllite");
            cleanDb2 = CreateNewDB(tempPath);

            Settings.Instance.DataRepositoryConnectionString = cleanDb2;
        }
        private Boolean SetupSQLLite()
        {
            string processor = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToString();
            if (processor.Contains("64"))
            {
                string sqldll = "System.Data.SQLite.DLL";
                string sqldll64 = "System.Data.SQLite64bit.DLL";
                File.Copy(sqldll64, sqldll, true);
                // Tried to force a load of x64 first, but it failed
                //Assembly.LoadFrom("System.Data.SQLite64bit.DLL"); 
                return true;
            }
            return true;
        }
        # region create db taken from (modified) main application
        /// <summary>
        /// Creates a new Data sqllite database
        /// </summary>
        /// <param name="newDbFileFullPath">path to the new db file</param>
        /// <returns>the db connection string</returns>
        public string CreateNewDB(string newDbFileFullPath)
        {
            SQLiteHelper.CreateSQLiteDatabase(newDbFileFullPath);
            FileInfo fi = new FileInfo(newDbFileFullPath);
            if (fi.Length == 0)
            {
                throw new FileNotFoundException("Cannot create new database '" + newDbFileFullPath + "'");
            }

            return SQLiteHelper.GetSQLiteConnectionString(newDbFileFullPath);
        }
       
        #endregion

        [Test]
        public void TestIntialize()
        {
            // and selection manager
            // and repository manager
            // and dbmanager
            HydroDatabase hdb = new HydroDatabase();
            hdb.ConnectionString = Settings.Instance.DataRepositoryConnectionString;

            Assert.NotNull(hdb);
            Assert.That(hdb.ConnectionString == Settings.Instance.DataRepositoryConnectionString);
            Assert.NotNull(hdb.RepositoryManager);
            Assert.That(hdb.DbOperations.ConnectionString == Settings.Instance.DataRepositoryConnectionString);

            Assert.NotNull(hdb.RepositoryManager.DbOperations == hdb.DbOperations);
        }
        [Test]
        public void TestIntialize1()
        {
            HydroDatabase hdb = new HydroDatabase(cleanDb1);


            Assert.NotNull(hdb);
            Assert.That(hdb.ConnectionString == cleanDb1);
            Assert.NotNull(hdb.RepositoryManager);
            Assert.That(hdb.DbOperations.ConnectionString == cleanDb1);

            Assert.NotNull(hdb.RepositoryManager.DbOperations == hdb.DbOperations);

        }
        [Test]
        public void TestIntialize2()
        {
            HydroDatabase hdb = new HydroDatabase(cleanDb1,DatabaseTypes.SQLite);


            Assert.NotNull(hdb);
            Assert.That(hdb.ConnectionString == cleanDb1);
            Assert.NotNull(hdb.RepositoryManager);
            Assert.That(hdb.DbOperations.ConnectionString == cleanDb1);

            Assert.NotNull(hdb.RepositoryManager.DbOperations == hdb.DbOperations);


        }

        [Test]
        public void DatabaseChangedEventTest()
        {
            HydroDatabase hdb = new HydroDatabase();
            int counter = 0;
            hdb.DatabaseChanged += (delegate
            {
                counter++;
            });
            hdb.ConnectionString = cleanDb1;
            Assert.That(counter == 1);

        }

        [Test]
        public void DatabaseChangedEventNotChangedTest()
        {
            HydroDatabase hdb = new HydroDatabase();
            int counter = 0;
            hdb.DatabaseChanged += (delegate
            {
                counter++;
            });
            hdb.ConnectionString = hdb.ConnectionString;
            Assert.That(counter == 0);
        }

        [Test]
        public void LoadTable()
        {
            HydroDatabase hdb = new HydroDatabase();
            hdb.ConnectionString = Settings.Instance.DataRepositoryConnectionString;
            // this should come from a constant set of queries
            string sqlTest = "SELECT * FROM DataThemeDescriptions";
            DataTable dt = hdb.LoadTable(sqlTest);
            Assert.NotNull(dt);
            Assert.That(dt.Columns.Contains("ThemeId"));

        }

        [Test]
        /* Generic exception from Nunit
         * ...which passes only if it throws the specified exception.	
         * If it throws a different exception then the test will fail. 
         * This is true even if the thrown exception inherits from the expected exception.
        // System.Data.Sqlite exceptions are of type System.Data.Common.DbException
       // [ExpectedException(typeof(System.Data.Common.DbException))]
         *//*
        [ExpectedException]
        public void LoadTableBadQuery()
        {
            HydroDatabase hdb = new HydroDatabase();
            // this should come from a constant set of queries
            string sqlTest = "SELECT * FROM JABBERQOKCY";
            DataTable dt = hdb.LoadTable(sqlTest);
            Assert.Null(dt);
        }*/
    }
    
}
