using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace HydroDesktop.Database.Tests.DatabaseTests
{
    [TestFixture]
    class SqlLiteHelperTest
    {
        private readonly List<string> _tempFiles = new List<string>();

        [TearDown]
        public void Clear()
        {
            foreach(var file in _tempFiles)
            {
                try
                {
                    File.Delete(file);
                }catch
                {
                }
            }
        }

        [Test]
        [TestCase(DatabaseType.DefaulDatabase)]
        [TestCase(DatabaseType.MetadataCacheDatabase)]
        public void CheckDatabaseSchema_ValidDatabases_Test(DatabaseType databaseType)
        {
            var target = Path.GetTempFileName();
            _tempFiles.Add(target);

            switch(databaseType)
            {
                case DatabaseType.DefaulDatabase:
                    SQLiteHelper.CreateSQLiteDatabase(target);
                    break;
                case DatabaseType.MetadataCacheDatabase:
                    SQLiteHelper.CreateMetadataCacheDb(target);
                    break;
            }
            
            Assert.DoesNotThrow(() => SQLiteHelper.CheckDatabaseSchema(target, databaseType));
        }

        [Test]
        [TestCase("TestDatabases\\defaultDatabase_WithoutSomeTables.sqlite", "Table 'Points' not found")]
        [TestCase("TestDatabases\\defaultDatabase_WithoutSomeColumns.sqlite", "Table 'Points': column 'PointType' not found")]
        public void CheckDatabaseSchema_NotValidDatabases_Test(string localPathToDb, string exMessage)
        {
            var target = Path.Combine(Environment.CurrentDirectory, localPathToDb);

            var exceptionThrows = false;
            try
            {
                SQLiteHelper.CheckDatabaseSchema(target, DatabaseType.DefaulDatabase);
            }
            catch (InvalidDatabaseSchemaException ex)
            {
                exceptionThrows = true;
                Assert.IsTrue(ex.Message.Contains(exMessage));
            }
            Assert.IsTrue(exceptionThrows);
        }
    }
}
