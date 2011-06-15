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
            DbWriter his = new DbWriter();

            IArgument[] arguments = new IArgument[5];
            arguments[0] = new Argument("DbPath", @"..\data\cuahsi-his\demo.db", true, "Database");
            arguments[1] = new Argument("Organization","University of South Carolina",true,"");
            arguments[2] = new Argument("Address","none",true,"");
            arguments[3] = new Argument("Citation","none",true,"");
            arguments[4] = new Argument("City", "none",true,"");

            Dictionary<string, string> db_args = his.ReadDbArgs(arguments);

        }

        
    }
}
