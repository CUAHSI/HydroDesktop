using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace SMW.Unit_Test
{
    [TestFixture]
    public class ODM_Tests
    {
        SMW.ODM odmUtils = new ODM();

        [Test]
        public void ODM_AddDef()
        {
        }

        [Test]
        public void ODM_CreateCsv()
        {
            Console.WriteLine("\n Writing ODM .csv file....");
            //Create DateTimes and Values arraylists
            System.Collections.ArrayList Dates = new System.Collections.ArrayList();
            System.Collections.ArrayList Vals = new System.Collections.ArrayList();
            DateTime time = DateTime.Now;
            for (int i = 0; i <= 10; i++)
            {
                Dates.Add(time.AddHours((double)i));
                Vals.Add(Math.Pow(2, i));

            }
            //Define ODM Parameters
            odmUtils.DateTimes  = Dates;
            odmUtils.Values     = Vals;
            odmUtils.SiteName = "TEST";
            odmUtils.SiteCode = "45";
            odmUtils.CSVPath = System.Environment.CurrentDirectory;
            odmUtils.CreateODMcsv();

            Console.WriteLine("ODM .csv has been written");

        }

        [Test]
        public void ODM_CreateBat()
        {
            Console.WriteLine("\n Writing ODM .bat file....");

            odmUtils.SiteName = "TEST";
            odmUtils.CSVPath = System.Environment.CurrentDirectory;

            odmUtils.CreateBat();

            Console.WriteLine("ODM .bat has been written");

        }

        [Test]
        public void ODM_LoadBat()
        {

            Console.WriteLine("\n Loading .bat file using ODM_Data_Loader");

            odmUtils.SiteName = "TEST";
            odmUtils.CSVPath = System.Environment.CurrentDirectory;

            odmUtils.LoadIntoODM();


            Console.WriteLine(" Data has been loaded into ODM ");
        }


    }

    [TestFixture]
    public class Utilities_Tests
    {
    
    }

    [TestFixture]
    public class Wrapper_Tests
    {
        mock_class mock;

        [Test]
        public void Test_Path()
        {
            //create a new instance of the mock class
            mock = new mock_class();

            //set variables using a faulty config path
            string path = System.IO.Directory.GetCurrentDirectory() + "\\some_omi.omi";
            try
            {
                //this will always fail
                mock.SetVariablesFromConfigFile(path);
            }
            catch (SystemException) { };

            //get the path name that was set
            string p = mock.PATH;
            
            //remove the omi filename from the end of the "path" string
            string[] array = path.Split('\\');
            Array.Resize<string>(ref array, array.Length - 1);
            path = null;
            foreach (string s in array)
                path += s + "\\";

            Assert.IsTrue(p == path, "The assigned path is different from the saved path!");



            
        }
    }
    public class mock_class : SMW.Wrapper
    {
        public override void Initialize(System.Collections.Hashtable properties) { }
        public override bool PerformTimeStep(){return true;}
        public override void Finish(){}
    }

}