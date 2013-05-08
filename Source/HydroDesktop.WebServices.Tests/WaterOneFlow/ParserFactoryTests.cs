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
            var target = new ParserFactory();

            var wof10 = new DataServiceInfo {Version = 1.0};
            var parser10 = target.GetParser(wof10);
            Assert.IsTrue(parser10 is WaterOneFlow10Parser);

            var wof11 = new DataServiceInfo { Version = 1.1 };
            var parser11 = target.GetParser(wof11);
            Assert.IsTrue(parser11 is WaterOneFlow11Parser);
        }
    }
}
