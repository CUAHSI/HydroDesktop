using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroShare.Properties;

namespace HydroShare
{
    public class HydroSharePlugin : Extension
    {
        #region Fields

        private SimpleActionItem test1;
        private SimpleActionItem test2;

        private readonly string _hydroShareKey = SharedConstants.HydroShareRootkey;

        #endregion

        #region Plugin operations

        public override void Activate()
        {
           // AddHydroShareRibbon();
          //  base.Activate();

           //  App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;   
        }

        public override void Deactivate()
        {
       
           // App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;
         

            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        #endregion

        private void AddHydroShareRibbon()
        {
            var head = App.HeaderControl;
            head.Add(new RootItem(_hydroShareKey, Resources.HydroShare) { SortOrder = 180 });

            test1 = new SimpleActionItem(_hydroShareKey, "", test_Click) { LargeImage = Resources.satisfied, GroupCaption = "Happy/Sad", Visible = true };
            test2 = new SimpleActionItem(_hydroShareKey, "", test_Click) { LargeImage = Resources.sad, GroupCaption = "Happy/Sad", Visible = false };  
           
            head.Add(test1);
            head.Add(test2);
         
        }


        void test_Click(object sender, EventArgs e)
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

        }
       
    }
}
