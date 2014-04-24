using HydroDesktop.WebServices.WaterML;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterML
{
    [TestFixture]
    public class ParserHelperTests
    {
        [Test]
        [TestCase("+7:30", 7.5)]
        [TestCase("-7:30", -7.5)]
        [TestCase("-7:00", -7)]
        public void ConvertUtcOffset(string str, double expected)
        {
            var actual = ParserHelper.ConvertUtcOffset(str);
            Assert.AreEqual(expected, actual);
        }
    }
}