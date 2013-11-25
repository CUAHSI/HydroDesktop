using System;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow20ParserTests
    {
        [Test]
        public  void ParseDataValuesV2()
        {
            var xmlPath = @"TestFiles\v20\GetValues_Mendon_usu3_wml2.xml";
            var target = new WaterOneFlow20Parser();

            var result = target.ParseGetValues(xmlPath);
            var series = result[0];

            // Check first data value
            var first = series.DataValueList[0];
            Assert.AreEqual(12.91645, first.Value);
            Assert.AreEqual(new DateTime(2005, 08, 05, 07, 00, 00), first.DateTimeUTC);
            Assert.AreEqual(new DateTime(2005, 08, 05, 00, 00, 00), first.LocalDateTime);
            Assert.AreEqual(-7, first.UTCOffset);

            // Check last data value
            var last = series.DataValueList[series.DataValueList.Count - 1];
            Assert.AreEqual(12.95674, last.Value);
            Assert.AreEqual(new DateTime(2005, 08, 06, 07, 00, 00), last.DateTimeUTC);
            Assert.AreEqual(new DateTime(2005, 08, 06, 00, 00, 00), last.LocalDateTime);
            Assert.AreEqual(-7, last.UTCOffset);

            // Check series  BeginDateTime/BeginDateTimeUTC, EndDateTime/EndDateTimeUTC
            var expectedBeginDateTime = first.LocalDateTime;
            var expectedBeginDateTimeUTC = first.DateTimeUTC;

            var expectedEndDateTime = last.LocalDateTime;
            var expectedEndDateTimeUTC = last.DateTimeUTC;

            Assert.AreEqual(expectedBeginDateTime, series.BeginDateTime);
            Assert.AreEqual(expectedBeginDateTimeUTC, series.BeginDateTimeUTC);
            Assert.AreEqual(expectedEndDateTime, series.EndDateTime);
            Assert.AreEqual(expectedEndDateTimeUTC, series.EndDateTimeUTC);
        }
    }
}
