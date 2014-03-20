using System;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterML
{
    [TestFixture]
    public class ParserFactoryTests
    {
        [Test]
        [TestCase(1.0, typeof(WaterML10Parser))]
        [TestCase(1.1, typeof(WaterML11Parser))]
        [TestCase(2.0, typeof(WaterML20Parser))]
        public void GetParser(double version, Type type)
        {
            var wof = new DataServiceInfo {Version = version};
            var parser = ParserFactory.GetParser(wof);
            Assert.AreEqual(type, parser.GetType());
        }
    }
}
