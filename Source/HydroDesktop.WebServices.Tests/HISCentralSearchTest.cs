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
    public class HISCentralSearchTest
    {
        //private string _url1 = @"http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";
        private string _url = @"http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        
        
        [Test]
        public void Test_GetOntologyTree()
        {
            HISCentralClient client = new HISCentralClient(_url);
            client.GetOntologyTree();
        }
        

        /// <summary>
        /// Can we find any Nitrate Nitrogen data in the lower Chesapeke Bay HUC?
        /// </summary>
        [Test]
        public void ChesapekeBay_NitrateNitrogen()
        {
            
            
            double xMin = -112.147;
            double xMax = -111.388;
            double yMin = 41.370;
            double yMax = 42.002;
            DateTime beginDate = new DateTime(1950,1,1);
            DateTime endDate = new DateTime(2011, 1, 1);

            Box testBox = new Box(xMin, xMax, yMin, yMax);

            //HISCentralClient client1 = new HISCentralClient(_url1);
            //IList<SeriesMetadata> seriesList = client1.GetSeriesCatalogForBox(testBox, null, beginDate, endDate);
            //Assert.Greater(seriesList.Count, 0, "The area + {" + testBox.ToString() + "} should have 1 or more sites.");
        }
    }
}
