using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.WebServices;
using HydroDesktop.DataModel;
using HydroDesktop.Database;
using NUnit.Framework;


namespace WebServiceTest
{
    [TestFixture]
    public class SaveValuesTest
    {
        [Test]
        public void TestSaveMultipleSeries()
        {
            string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "hydrodesktop");
            string[] files = System.IO.Directory.GetFiles(tempDir);
            Array.Sort(files);

            IWaterOneFlowParser parser = new WaterOneFlow10Parser();

            Theme theme1 = new Theme("SaveValuesTest1", "Test on Save Values");

            ActualDataManager manager = HydroDesktop.Database.Config.ActualDataManager();

            foreach (string fileName in files)
            {
                IList<Series> seriesList = parser.ParseGetValues(fileName);
                Series series1 = seriesList[0];

                manager.SaveSeries(series1, theme1, OverwriteOptions.Copy);
            }
        }
    }
}
