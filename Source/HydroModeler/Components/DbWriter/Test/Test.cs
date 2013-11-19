using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CUAHSI.HIS;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Buffer;
using System.Collections;
using System.Windows.Forms;
using RandomInputGenerator;

namespace CUAHSI.HIS.Test
{
    [TestFixture]
    public class HISOpenMIComponentTests
    {
        [Test]
        public void GetComponentDescription()
        {
            Console.Write("Begin Get Component Description Test...");
            DbWriter his = new DbWriter();
            IArgument[] arguments = new IArgument[1];
            arguments[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            his.Initialize(arguments);

            Console.WriteLine("component description: " + his.ComponentDescription);
        }

        [Test]
        public void GetModelDescription()
        {
            Console.Write("Begin Get Model Description Test...");
            DbWriter his = new DbWriter();
            IArgument[] arguments = new IArgument[1];
            arguments[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            his.Initialize(arguments);

            Console.WriteLine("model description: " + his.ModelDescription);
        }

        /// <summary>
        /// This method tests hydrolinks ability get values
        /// </summary>
        [Test]
        public void GetValues()
        {
            //Console.Write("Begin Get Values Test...");
            ////create the his component
            //DbWriter dbwriter = new DbWriter();
            //IArgument[] arguments = new IArgument[2];
            //arguments[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            //arguments[1] = new Argument("Relaxation", "1", true, "Time interpolation factor, btwn 0(linear inter.) and 1(nearest neigbor)");
            //dbwriter.Initialize(arguments);

            //RandomInputGenerator.InputGenerator rand = new InputGenerator();
            //IArgument[] arg = new IArgument[1];
            //arg[0] = new Argument("ElementCount", "1", true, "");
            //rand.Initialize(arg);

            //////create dbreader component
            ////DbReader dbreader = new DbReader();
            ////IArgument[] arguments2 = new IArgument[2];
            ////arguments2[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            ////arguments2[1] = new Argument("Relaxation", "1", true, "Time interpolation factor, btwn 0(linear inter.) and 1(nearest neigbor)");
            ////dbreader.Initialize(arguments2);

            ////create a trigger component
            //Trigger trigger = new Trigger();
            //trigger.Initialize(null);

            ////link the components
            //Link link = new Link();
            //link.ID = "link-1";
            //link.TargetElementSet = dbwriter.GetInputExchangeItem(0).ElementSet;
            //link.TargetQuantity = dbwriter.GetInputExchangeItem(0).Quantity;
            //link.TargetComponent = dbwriter;
            //link.SourceElementSet = rand.GetOutputExchangeItem(0).ElementSet;
            //link.SourceQuantity = rand.GetOutputExchangeItem(0).Quantity;
            //link.SourceComponent = rand;
            //dbwriter.AddLink(link);
            //Link link2 = new Link();
            //link2.ID = "link-2";
            //link2.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;
            //link2.TargetQuantity = trigger.GetInputExchangeItem(0).Quantity;
            //link2.TargetComponent = trigger;
            //link2.SourceElementSet = rand.GetOutputExchangeItem(0).ElementSet;
            //link2.SourceQuantity = rand.GetOutputExchangeItem(0).Quantity;
            //link2.SourceComponent = dbwriter;
            //dbwriter.AddLink(link2);

            ////prepare 
            //rand.Prepare();
            //dbwriter.Prepare();

            //DateTime dt = Convert.ToDateTime("2009-08-20");

            //while (dt <= Convert.ToDateTime("2009-08-21"))
            //{
            //    Application.DoEvents();
            //    ITimeStamp time_stmp = new TimeStamp(CalendarConverter.Gregorian2ModifiedJulian(dt));
            //    ScalarSet scalarset = (ScalarSet)trigger.GetValues(time_stmp, "link-2");
            //    Console.WriteLine("GetValues: " + dt.ToString("s"));
            //    int i = 0;
            //    foreach (double d in scalarset.data)
            //    {
            //        Console.WriteLine(link.SourceElementSet.GetElementID(i).ToString() + " " + d.ToString());
            //        ++i;
            //    }
            //    dt = dt.AddMinutes(5);
            //}
            //dbwriter.Finish();
            //Console.Write("done. \n");
        }

        /// <summary>
        /// This method tests hydrolinks ability to construct a time horizon by reading the waterml database
        /// </summary>
        [Test]
        public void GetTimeHorizon()
        {
            Console.WriteLine("Begin Get Time Horizon Test...");
            //create the his component
            DbWriter his = new DbWriter();
            IArgument[] arguments = new IArgument[1];
            arguments[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            his.Initialize(arguments);

            //get earliest and latest times
            Console.WriteLine("start: " + CalendarConverter.ModifiedJulian2Gregorian(his.TimeHorizon.Start.ModifiedJulianDay).ToString());
            Console.WriteLine("end: " + CalendarConverter.ModifiedJulian2Gregorian(his.TimeHorizon.End.ModifiedJulianDay).ToString());
        }

        [Test]
        public void ReadOmiArgs()
        {
            DbWriter dbwriter = new DbWriter();
            RandomInputGenerator.InputGenerator random = new InputGenerator();

            //---- initialize random input generator
            IArgument[] arg = new IArgument[1];
            arg[0] = new Argument("ElementCount", "1", true, "");
            random.Initialize(arg);

            //---- initialize dbwriter
            string myCustomMethod = "Simulation 1, m=10, Tmax=100000";
            string myVariableName = "millimeters per second";
            string mySourceContact = "Tony";
            string mySourceDesc = "Test dource description";
            IArgument[] arguments = new IArgument[17];
            arguments[0] = new Argument("DbPath",@".\example4.sqlite",true,"");
            arguments[1] = new Argument("Variable.UnitName",myVariableName,true,"");
            arguments[2] = new Argument("Variable.UnitAbbr","m^3/s",true,"");
            arguments[3] = new Argument("Variable.UnitType","Flow",true,"");        
            arguments[4] = new Argument("Time.UnitName","second",true,"");
            arguments[5] = new Argument("Time.UnitAbbr","s",true,"");
            arguments[6] = new Argument("Time.UnitType","Time",true,"");
            arguments[7] = new Argument("Method.Description",myCustomMethod,true,"");
            arguments[8] = new Argument("Source.Organization","University of South Carolina",true,"");
            arguments[9] = new Argument("Source.Address","300 Main St.",true,"");
            arguments[10] = new Argument("Source.City","Columbia",true,"");
            arguments[11] = new Argument("Source.State","SC",true,"");
            arguments[12] = new Argument("Source.Zip","29206",true,"");
            arguments[13] = new Argument("Source.Contact",mySourceContact,true,"");
            arguments[14] = new Argument("Variable.Category","  ",true,""); //intentionally left blank
            arguments[15] = new Argument("Variable.SampleMedium","Surface Water",true,"");
            arguments[16] = new Argument("Source.Description", mySourceDesc, true, "");

            dbwriter.Initialize(arguments);

            //---- link the components
            Link link = new Link();
            link.ID = "link-1";
            link.TargetElementSet = dbwriter.GetInputExchangeItem(0).ElementSet;
            link.TargetQuantity = dbwriter.GetInputExchangeItem(0).Quantity;
            link.TargetComponent = dbwriter;
            ElementSet eset = new ElementSet("rand element set", "r_eset", ElementType.XYPoint, new SpatialReference("1"));
            Element e = new Element("1");
            e.AddVertex(new Vertex(1, 1, 0));
            eset.AddElement(e);
            link.SourceElementSet = eset;
            link.SourceQuantity = random.GetOutputExchangeItem(0).Quantity;
            link.SourceComponent = random;
            dbwriter.AddLink(link);

            //---- get the series info
            HydroDesktop.Interfaces.ObjectModel.Series series = dbwriter.serieses["r_eset_RandomInputGenerator_loc0"];

            //---- check that omi values were set properly
            Assert.IsTrue(series.Method.Description == myCustomMethod);
            Assert.IsTrue(series.Variable.VariableUnit.Name == myVariableName);
            Assert.IsTrue(series.Source.ContactName == mySourceContact);
            Assert.IsTrue(series.Variable.GeneralCategory == "Hydrology", "blank argument is not ignored!!");
            Assert.IsTrue(series.Source.Description == mySourceDesc);
            

        }

        
    }
}
