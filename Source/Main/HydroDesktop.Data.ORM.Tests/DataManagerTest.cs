using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Data.ORM.Tests
{
    [TestFixture]
    public class DataManagerTest
    {
        [Test]
        public void TestDataManager1()
        {
            RepositoryManager manager = TestConfig.RepositoryManager;
            IList<Theme> themeLst = manager.GetAllThemes();

        }

        [Test]
        public void TestMetadataCacheManager()
        {
            MetadataCacheManagerSQL manager = TestConfig.MetadataCacheManager;
            IList<DataServiceInfo> services = manager.GetAllServices();
            //string serviceUrl = services[0].ServiceEndpointURL;
        }

        [Test]
        public void TestBothManagers()
        {
            string conString1 = Config.DefaultLocalCacheConnection();
            MetadataCacheManagerSQL manager1 = TestConfig.MetadataCacheManager;   
            RepositoryManagerSQL manager2 = TestConfig.SQLRepositoryManager;
                
            IList<DataServiceInfo> services = manager1.GetAllServices();
            IList<Theme> themes = manager2.GetAllThemes();
        }
    }
}