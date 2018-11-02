using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenMI.Standard;
using System.Collections;
using LoadCalculator;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using System.Xml;
using System.Diagnostics;

namespace Test
{
    [TestFixture]
    public class Test_Class
    {
        LoadCalculator.LoadCalculatorLinkableEngine _engine;

        [TestFixtureSetUp]
        public void Test_Setup()
        {
            
        }

        [Test]
        public void ExchangeItemsDefinedByConfig()
        {
            _engine = new LoadCalculator.LoadCalculatorLinkableEngine();

            ArrayList componentArguments = new ArrayList();
            componentArguments.Add(new Argument("ConfigFile","./config.xml",true,"none"));
            _engine.Initialize((IArgument[])componentArguments.ToArray(typeof(IArgument)));
            int in_count = _engine.InputExchangeItemCount;
            int out_count = _engine.OutputExchangeItemCount;

            for (int i = 0; i <= in_count - 1; i++)
            {
                InputExchangeItem ie = (InputExchangeItem)_engine.GetInputExchangeItem(i);
                Debug.Write("Testing Input Element Count...");Assert.IsFalse(ie.ElementSet.ElementCount > 0);Debug.WriteLine("done.");
                Debug.Write("Testing Input Conv2SI...");Assert.IsTrue(ie.Quantity.Unit.ConversionFactorToSI >= 0);Debug.WriteLine("done.");
                Debug.Write("Testing Input Offset2SI...");Assert.IsTrue(ie.Quantity.Unit.OffSetToSI >= 0);Debug.WriteLine("done.");
            }

            for (int i = 0; i <= out_count - 1; i++)
            {
                OutputExchangeItem oe = (OutputExchangeItem)_engine.GetOutputExchangeItem(i);
                Debug.Write("Testing Output Element Count...");Assert.IsTrue(oe.ElementSet.ElementCount > 0);Debug.WriteLine("done.");
                Debug.Write("Testing Output Conv2SI...");Assert.IsTrue(oe.Quantity.Unit.ConversionFactorToSI >= 0);Debug.WriteLine("done.");
                Debug.Write("Testing Output Offset2SI...");Assert.IsTrue(oe.Quantity.Unit.OffSetToSI >= 0);Debug.WriteLine("done.");
            }

            Assert.IsTrue(_engine.TimeHorizon.Start.ModifiedJulianDay == CalendarConverter.
                            Gregorian2ModifiedJulian(new DateTime(2009, 10, 27, 08, 30, 00)));

            _engine.Finish();
                
        }
        [Test]
        public void ExchangeItemsDefinedByOmi()
        {
            _engine = new LoadCalculator.LoadCalculatorLinkableEngine();

            ArrayList componentArguments = new ArrayList();
            componentArguments.Add(new Argument("StartDateTime", "10/27/2009 8:30:00AM", true, "none"));
            componentArguments.Add(new Argument("TimeStepInSeconds", "86400", true, "none"));
            componentArguments.Add(new Argument("InputTimeSeries", "Discharge:[cms]", true, "none"));
            componentArguments.Add(new Argument("InputTimeSeries", "Concentration:[mg/l]", true, "none"));
            componentArguments.Add(new Argument("OutputTimeSeries", "GillsCreek:Nitrogen Loading:[kg/day]:NumElements=1", true, "none"));

            _engine.Initialize((IArgument[])componentArguments.ToArray(typeof(IArgument)));

            int in_count = _engine.InputExchangeItemCount;
            int out_count = _engine.OutputExchangeItemCount;

            for (int i = 0; i <= in_count - 1; i++)
            {
                InputExchangeItem ie = (InputExchangeItem)_engine.GetInputExchangeItem(i);
                Debug.Write("Testing Input Element Count..."); Assert.IsFalse(ie.ElementSet.ElementCount > 0); Debug.WriteLine("done.");
                Debug.Write("Testing Input Conv2SI..."); Assert.IsTrue(ie.Quantity.Unit.ConversionFactorToSI >= 0); Debug.WriteLine("done.");
                Debug.Write("Testing Input Offset2SI..."); Assert.IsTrue(ie.Quantity.Unit.OffSetToSI >= 0); Debug.WriteLine("done.");
            }

            for (int i = 0; i <= out_count - 1; i++)
            {
                OutputExchangeItem oe = (OutputExchangeItem)_engine.GetOutputExchangeItem(i); 
                Debug.Write("Testing Output Element Count..."); Assert.IsTrue(oe.ElementSet.ElementCount > 0); Debug.WriteLine("done.");
                Debug.Write("Testing Output Conv2SI..."); Assert.IsTrue(oe.Quantity.Unit.ConversionFactorToSI >= 0); Debug.WriteLine("done.");
                Debug.Write("Testing Output Offset2SI..."); Assert.IsTrue(oe.Quantity.Unit.OffSetToSI >= 0); Debug.WriteLine("done.");
            }

            Assert.IsTrue(_engine.TimeHorizon.Start.ModifiedJulianDay == CalendarConverter.
                            Gregorian2ModifiedJulian(new DateTime(2009, 10, 27, 08, 30, 00)));

            _engine.Finish();
        }
        [Test]
        public void IdBasedElementSet_DefinedInConfig()
        {
            _engine = new LoadCalculator.LoadCalculatorLinkableEngine();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("./config.xml");
            XmlElement root = doc.DocumentElement;
            XmlNodeList outputExchangeItems = root.SelectNodes("//OutputExchangeItem");
            foreach(XmlNode exchangeItem in outputExchangeItems)
            {
                XmlNode elementSet = exchangeItem.SelectSingleNode("ElementSet");
                XmlNode numelem = doc.CreateElement("NumberOfElements");
                numelem.InnerText = "10";
                elementSet.AppendChild(numelem);
            }

            doc.Save("./config_temp.xml");
            

            

            ArrayList componentArguments = new ArrayList();
            componentArguments.Add(new Argument("ConfigFile", "./config_temp.xml", true, "none"));
            _engine.Initialize((IArgument[])componentArguments.ToArray(typeof(IArgument)));

            int out_count = _engine.OutputExchangeItemCount;

            for (int i = 0; i <= out_count - 1; i++)
            {
                OutputExchangeItem oe = (OutputExchangeItem)_engine.GetOutputExchangeItem(i);
                Debug.Write("Testing Element Count..."); Assert.IsTrue(oe.ElementSet.ElementCount == 10); Debug.WriteLine("done.");
                Debug.Write("Testing Element Type..."); Assert.IsTrue(oe.ElementSet.ElementType == ElementType.IDBased); Debug.WriteLine("done.");
                Debug.Write("Testing Element Conv2SI..."); Assert.IsTrue(oe.Quantity.Unit.ConversionFactorToSI >= 0); Debug.WriteLine("done.");
                Debug.Write("Testing Element Offset2SI..."); Assert.IsTrue(oe.Quantity.Unit.OffSetToSI >= 0); Debug.WriteLine("done.");
            }

            Assert.IsTrue(_engine.TimeHorizon.Start.ModifiedJulianDay == CalendarConverter.
                            Gregorian2ModifiedJulian(new DateTime(2009, 10, 27, 08, 30, 00)));

            doc = null;

            System.IO.File.Delete("./config_temp.xml");

            _engine.Finish();
        }

    }
}
