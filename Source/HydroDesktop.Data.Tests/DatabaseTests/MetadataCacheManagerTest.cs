using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Database;
using HydroDesktop.WebServices;
using NUnit.Framework;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Data.Tests.DatabaseTests
{
    [TestFixture]
    public class MetadataCacheManagerTest
    {
        [Test]
        public void CanReadDataServices()
        {
            //DbOperations dbTools = new DbOperations(Config.DefaultLocalCacheConnection(), DatabaseTypes.SQLite);

            MetadataCacheManagerSQL manager = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, TestConfig.DefaultLocalCacheConnection);
            IList<DataServiceInfo>  services = manager.GetAllServices();
        }

      
        [Test]
        [Ignore("This takes too long")]
        public void CanDeleteRecordsForService()
        {
            MetadataCacheManagerSQL manager = TestConfig.MetadataCacheManager;

            IList<DataServiceInfo> services = manager.GetAllServices();
            foreach (DataServiceInfo serv in services)
            {
                int numDeleted = manager.DeleteRecordsForService(serv, false);
            }
        }
        
        [Test]
        public void CanSaveDataService()
        {
            Random random   = new Random();
            DataServiceInfo service = GeDatatService(random.Next());

            MetadataCacheManagerSQL manager = TestConfig.MetadataCacheManager;
            manager.SaveDataService(service);

            //Assert.Greater(service.Id, 0, "the id of saved service should be > 0");
        }

        private DataServiceInfo GeDatatService(int identifier)
        {
            string url = @"http://water.sdsc.edu/wateroneflow/NWIS/DailyValues.asmx";
            
            DataServiceInfo service = new DataServiceInfo(url, "nwis daily values");

            service.Id = identifier;
            service.DescriptionURL = url;
            service.ServiceTitle = "NWIS Daily values";
            service.ServiceCode = "NWISDV";
            service.ServiceType = "WaterOneFlow service";
            service.ServiceName = "WaterOneFlow";
            service.Protocol = "SOAP";
            service.Version = 1.1;
            service.NorthLatitude = 90.0;
            service.SouthLatitude = -90.0;
            service.EastLongitude = 180.0;
            service.WestLongitude = -180.0;
            service.ContactName = "unknown";
            service.ContactEmail = "unknown";
            service.Abstract = "unknown";
            service.Citation = "unknown";
            return service;
        }

        // This should indepented of the collection from a webservice
        // using data loaded from a file        
        [Ignore("Dynamic Compilation of services not working on build system")]        
        [Test]
        public void CanSaveOneSeries()
        {
            Random random = new Random();
            string url = @"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx";
           
            MetadataCacheManagerSQL manager = TestConfig.MetadataCacheManager;

            WaterOneFlowClient client = new WaterOneFlowClient(url);

            IList<Site> sites = client.GetSites();

            IList<SeriesMetadata> seriesList = client.GetSiteInfo(sites[0].Code);

            SeriesMetadata firstSeries = seriesList[0];

            DataServiceInfo service = GeDatatService(random.Next());
            manager.SaveDataService(service);

            firstSeries.DataService = service;

            manager.SaveSeries(firstSeries, service);
        }

        // This should indepented of the collection from a webservice
        // using data loaded from a file        
        [Ignore("Dynamic Compilation of services not working on build system")]   
        [Test]
        public void CanSaveMultipleSeries()
        {
            Random random = new Random();
            //string url1 = @"http://his02.usu.edu/littlebearriver/cuahsi_1_0.asmx";
            //string url2 = @"http://icewater.boisestate.edu/dcew2dataservices/cuahsi_1_0.asmx";
            string url3 = @"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx";

            MetadataCacheManagerSQL manager = TestConfig.MetadataCacheManager;

            WaterOneFlowClient client = new WaterOneFlowClient(url3);

            IList<Site> siteList = client.GetSites();

            IList<SeriesMetadata> seriesList = new List<SeriesMetadata>();

            DataServiceInfo service = GeDatatService(random.Next());
            manager.SaveDataService(service);

            foreach(Site site in siteList)
            {
                IList<SeriesMetadata> seriesList1 = client.GetSiteInfo(site.Code);
                foreach(SeriesMetadata series in seriesList1)
                {
                    seriesList.Add(series);
                }
            }

            foreach (SeriesMetadata series in seriesList)
            {
                manager.SaveSeries(series, service);
            }
        }
    }
}
