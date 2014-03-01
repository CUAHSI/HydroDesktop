using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class ParserFactoryTests
    {
        [Test]
        public void GetParser()
        {
            var wof10 = new DataServiceInfo {Version = 1.0};
            var parser10 = ParserFactory.GetParser(wof10);
            Assert.IsTrue(parser10 is WaterOneFlow10Parser);

            var wof11 = new DataServiceInfo { Version = 1.1 };
            var parser11 = ParserFactory.GetParser(wof11);
            Assert.IsTrue(parser11 is WaterOneFlow11Parser);

            var wof20 = new DataServiceInfo { Version = 2.0 };
            var parser20 = ParserFactory.GetParser(wof20);
            Assert.IsTrue(parser20 is WaterOneFlow20Parser);
        }
    }
}
