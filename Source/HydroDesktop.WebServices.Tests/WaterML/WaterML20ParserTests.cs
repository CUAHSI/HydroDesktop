using System;
using HydroDesktop.WebServices.WaterML;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterML
{
    [TestFixture]
    public class WaterML20ParserTests
    {
        [Test]
        [TestCase(@"TestFiles\v20\TimeSeries_1.xml", 145)]
        [TestCase(@"TestFiles\v20\TimeSeries_2.xml", 73)]
        [TestCase(@"TestFiles\v20\TimeSeries_3.xml", 145)]
        [TestCase(@"TestFiles\v20\GetValues_Mendon_usu3_wml2.xml", 49)]
        public void ParseGetValues(string xmlFile, int valuesCount)
        {
            var target = new WaterML20Parser();
            var series = target.ParseGetValues(xmlFile);
            Assert.IsTrue(series.Count > 0);
            Assert.AreEqual(valuesCount, series[0].DataValueList.Count);
        }
    }
}
