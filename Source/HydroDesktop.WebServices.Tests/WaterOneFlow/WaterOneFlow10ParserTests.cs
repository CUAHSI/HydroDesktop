using System;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;
using System.IO;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow10ParserTests
    {
        [TestCase(@"TestFiles\v10\")]
        public void TestParseDataSeries(String samplesPath)
        {
            var target = new WaterOneFlow10Parser();
            foreach (var fileName in Directory.GetFiles(samplesPath))
            {
                var seriesList = target.ParseGetValues(fileName);

                Assert.Greater(seriesList.Count, 0);
                Assert.Greater(seriesList[0].ValueCount, 0,
                               "Error in filename '" + fileName +
                               "' the number of values in the seriesList must be greater than zero.");
            }
        }


        [TestCase(@"TestFiles\v10sites\")]
        public void TestParseGetSites(String samplesPath)
        {
            var target = new WaterOneFlow10Parser();
            foreach (var fileName in Directory.GetFiles(samplesPath))
            {
                var sites = target.ParseGetSites(fileName);
                Assert.Greater(sites.Count, 0,
                               "Error in file '" + fileName + "' the number of sites must be greater than zero.");
            }
        }
    }
}
