using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroShare.Properties;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.IO;

namespace HydroShare
{
    public class HydroSharePlugin : Extension
    {
        #region Fields

        //private SimpleActionItem test1;
        //private SimpleActionItem test2;
        private SimpleActionItem pythonTest;

        private readonly string _hydroShareKey = SharedConstants.HydroShareRootkey;

        #endregion

        #region Plugin operations

        public override void Activate()
        {
           AddHydroShareRibbon();
           base.Activate();

           //App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;   
        }

        public override void Deactivate()
        {
       
           //App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;
         

           App.HeaderControl.RemoveAll();
           base.Deactivate();
        }

        #endregion

        private void AddHydroShareRibbon()
        {
            var head = App.HeaderControl;
            head.Add(new RootItem(_hydroShareKey, Resources.HydroShare) { SortOrder = 180 });

            //test1 = new SimpleActionItem(_hydroShareKey, "", test_Click) { LargeImage = Resources.satisfied, GroupCaption = "Happy/Sad", Visible = true };
            //test2 = new SimpleActionItem(_hydroShareKey, "", test_Click) { LargeImage = Resources.sad, GroupCaption = "Happy/Sad", Visible = false };
            pythonTest = new SimpleActionItem(_hydroShareKey, "", pythonTest_Click) { LargeImage = Resources.python_32x32, GroupCaption = "Python", Visible = true };  

            //head.Add(test1);
            //head.Add(test2);
            head.Add(pythonTest);
         
        }


        /*void test_Click(object sender, EventArgs e)
        {
            
            if (test1.Visible == true)
            {
                test2.Visible = true;
                test1.Visible = false;
            }
            else
            {
                test1.Visible = true;
                test2.Visible = false;
            }

        }*/

        void pythonTest_Click(object sender, EventArgs e)
        {

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"python";
            String cmd = @"C:\Plugins\Hydroshare\GetShapefilesFromHydroShareMyFrame1.py";
            String args = @"";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }

           // var ipy = Python.CreateRuntime();
           // dynamic test = ipy.ExecuteFile(@"C:\users\cuyler frisby\documents\python\test.py");
            //dynamic test = ipy.UseFile(@"C:\Users\Cuyler Frisby\Documents\Python\bagit-1.2.1\bagit.py");
            //test.make_bag("C:\\Users\\Cuyler Frisby\\Desktop\\NorthAmerica Bag Test", "None", 1);
            //test.Simple();

            /*ScriptRuntime ipy = Python.CreateRuntime();
            ScriptEngine engine = ipy.GetEngine("Python");
            var paths = engine.GetSearchPaths();
            paths.Add(@"C:\Program Files\IronPython 2.7");
            paths.Add(@"C:\Users\Student\Documents\HD-3\Binaries\Plugins\HydroShare\Lib");
            engine.SetSearchPaths(paths);


            dynamic test = ipy.ExecuteFile(System.IO.Path.Combine("Plugins", "HydroShare", "Lib", "GetShapefilesFromHydroShareMyFrame1.py"));

            test.test();
            //dynamic test = ipy.UseFile(@"C:\Users\Cuyler Frisby\Documents\Python\bagit-1.2.1\bagit.py");

            /*dynamic test = ipy.ExecuteFile(System.IO.Path.Combine("Plugins", "HydroShare", "Lib", "hydrosharedownload.py"));
            IList<object> originalResult = (IList<object>)test.retrieveList();
            List<string> typeSafeResult = new List<string>();
            foreach (object element in originalResult)
            {
                typeSafeResult.Add((string)element);
            }
            String x = typeSafeResult[0];*/
        }

        public class dynamic_demo
        {
            static void Main()
            {
            //    var ipy = Python.CreateRuntime();
               // dynamic test = ipy.UseFile("Test.py");
                //test.Simple();
            }
        }
       
    }
}
