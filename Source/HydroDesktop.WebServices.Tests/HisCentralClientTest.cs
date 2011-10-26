using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HydroDesktop.WebServices;
using HydroDesktop.Interfaces.ObjectModel;
using System.Data;

namespace HydroDesktop.WebServices.Tests
{
    [TestFixture]
    public class WaterOneFlowClientTest
    {
         private string _url1 = @"http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";
      // private string _url1 = @"http://localhost/hiscentral/webservices/hiscentral.asmx";
        private string _url2 = @"http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        
        [Test]
        [ExpectedException]
        public void TestInvalidURL()
        {
            string badURL = @"http://example.com";
            HISCentralClient client = new HISCentralClient(badURL);     
        }

        [Test]
        public void TestValidURL()
        {
            string goodURL = @"http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";
            HISCentralClient client = new HISCentralClient(goodURL);
            Assert.IsNotNull(client, "his central client should not be NULL");
        }

        [Test]
        public void Test_GetOntologyTree()
        {
            HISCentralClient client = new HISCentralClient(_url2);
            DataTable resultTable = client.GetOntologyTreeTable();
            Assert.Greater(resultTable.Rows.Count, 1000);
        }

        [Test]
        public void TestGetMappedVariables()
        {
            string goodURL = @"http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";
            HISCentralClient client = new HISCentralClient(goodURL);
            System.Data.DataTable mappedVariables = client.GetMappedVariables();
        }

        [Test]
        //Tests the GetSitesInBox() method for the Cache county, UTAH (this area definitely has some sites)
        public void TestGetSitesInBox()
        {
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            
            Box testBox = new Box(xMin, xMax, yMin, yMax);
            HISCentralClient client1 = new HISCentralClient(_url1);
            DataTable sitesInBox = client1.GetAllSitesInBox(testBox);
            Assert.Greater(sitesInBox.Rows.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites.");

            HISCentralClient client2 = new HISCentralClient(_url2);
            DataTable sitesInBox2 = client2.GetAllSitesInBox(testBox);
            Assert.Greater(sitesInBox2.Rows.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites.");

            //Assert.AreEqual(sitesInBox.Rows.Count, sitesInBox2.Rows.Count, "The old web service and new web service should return the same number of sites.");
        }

        [Test]
        //Tests the GetSitesInBox() method with the LittleBearRiver networkID specified for Cache County, UTAH
        public void TestGetSitesInBox_OneNetwork()
        {
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            int networkID = 52;

            Box testBox = new Box(xMin, xMax, yMin, yMax);

            HISCentralClient client1 = new HISCentralClient(_url1);
            DataTable sitesTable = client1.GetSitesInBox(testBox, string.Empty, new int[] { networkID });
            Assert.Greater(sitesTable.Rows.Count, 0, "The network " + networkID.ToString() + "should have some sites in the area + {" + testBox.ToString() + "}");
        }

        /// <summary>
        /// GetSeriesCatalogForBox, only the Box parameter is specified.
        /// </summary>
        [Test]
        public void GetSeriesCatalogForBox_NoParameters()
        {
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            DateTime beginDate = new DateTime(1950,1,1);
            DateTime endDate = new DateTime(2011, 1, 1);

            Box testBox = new Box(xMin, xMax, yMin, yMax);

            HISCentralClient client1 = new HISCentralClient(_url1);
            IList<SeriesMetadata> seriesList = client1.GetSeriesCatalogForBox(testBox, null, beginDate, endDate);
            Assert.Greater(seriesList.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites.");
        }

        /// <summary>
        /// GetSeriesCatalogForBox, the Box and one networkID is specified.
        /// </summary>
        [Test]
        public void GetSeriesCatalogForBox_OneNetwork()
        {
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            DateTime beginDate = new DateTime(1950, 1, 1);
            DateTime endDate = new DateTime(2011, 1, 1);
            int networkID = 52; //service ID of the LittleBearRiver

            Box testBox = new Box(xMin, xMax, yMin, yMax);

            HISCentralClient client1 = new HISCentralClient(_url1);
            IList<SeriesMetadata> seriesList = client1.GetSeriesCatalogForBox(testBox, null, beginDate, endDate, new int[]{networkID});
            Assert.Greater(seriesList.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites from LittleBearRiver network.");
        }

        /// <summary>
        /// GetSeriesCatalogForBox, the Box and one networkID is specified.
        /// </summary>
        [Test]
        public void GetSeriesCatalogForBox_TwoNetworks()
        {
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            DateTime beginDate = new DateTime(1950, 1, 1);
            DateTime endDate = new DateTime(2011, 1, 1);
            int BearNetworkID = 52; //service ID of the LittleBearRiver
            int USGSNetworkID = 1; //service(network) ID of USGS NWIS Daily values

            Box testBox = new Box(xMin, xMax, yMin, yMax);

            HISCentralClient client1 = new HISCentralClient(_url1);
            IList<SeriesMetadata> seriesList = client1.GetSeriesCatalogForBox(testBox, null, beginDate, endDate, new int[] { BearNetworkID, USGSNetworkID });
            Assert.Greater(seriesList.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites from LittleBearRiver network.");
        }
    }
}
