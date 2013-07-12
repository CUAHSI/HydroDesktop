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
         

            //App.HeaderControl.RemoveAll();
            //base.Deactivate();
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
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.ExecuteFile(@"C:\users\cuyler frisby\documents\python\test.py");
            //dynamic test = ipy.UseFile(@"C:\Users\Cuyler Frisby\Documents\Python\bagit-1.2.1\bagit.py");
            //test.make_bag("C:\\Users\\Cuyler Frisby\\Desktop\\NorthAmerica Bag Test", "None", 1);
            //test.Simple();
        }

        public class dynamic_demo
        {
            static void Main()
            {
                var ipy = Python.CreateRuntime();
                dynamic test = ipy.UseFile("Test.py");
                test.Simple();
            }
        }
       
    }
}
