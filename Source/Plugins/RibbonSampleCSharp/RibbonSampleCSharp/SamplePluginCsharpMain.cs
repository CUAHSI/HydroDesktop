using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using System.ComponentModel.Composition;
using SeriesView;
using DotSpatial.Controls.Header;

namespace RibbonSamplePlugin
{
    /// <summary>
    /// The main plugin class
    /// </summary>
    public class SamplePluginCsharpMain : Extension
    {
        #region Variables

        //reference to the main series view panel
        [Import("SeriesViewControl", typeof(SeriesViewControl))]
        private SeriesViewControl _seriesView;

        //the name of the plugin displayed in the ribbon tab
        private const string _pluginName = "Ribbon Sample Plugin";
        private const string HydroCsharpDockKey = "kDockRibbonSampleCsharp";
        private const string KHydroCSharp = "kRootRibbonSampleCsharp";

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            // Remove ribbon tab
            App.HeaderControl.RemoveItems();
            
            //remove the plugin panel           
            _seriesView.RemovePanel(_pluginName);          
            
            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        /// <summary>
        /// Activates the plug-in
        /// </summary>
        public override void Activate()
        {
            // Add "Ribbon" button to the "View" Panel in "Home" ribbon tab
            App.HeaderControl.Add(new RootItem(KHydroCSharp, _pluginName));

            var simpleButton = new SimpleActionItem("C# Sample", rb_Click);
            simpleButton.LargeImage = CreateCircleImage(Color.Blue);
            simpleButton.RootKey = KHydroCSharp;
            App.HeaderControl.Add(simpleButton);


            // Add a dockable panel
            MyUserControl uc = new MyUserControl(_seriesView.SeriesSelector);
            App.DockManager.Add(HydroCsharpDockKey, _pluginName, uc, DockStyle.Fill);

            // Create the Ribbon Button with a ribbon panel on the new ribbon tab        
            var simpleButton2 = new SimpleActionItem("ribbon button", this.rb_Click);
            simpleButton2.RootKey = KHydroCSharp;
            simpleButton2.LargeImage = CreateCircleImage(Color.Blue);
            simpleButton2.GroupCaption = "ribbon panel";
            App.HeaderControl.Add(simpleButton2);

            //ribbon panel caption
            var groupCaption2 = "ribbon panel 2";

            //Add a Ribbon Combo Box
            var simpleDropDown = new DotSpatial.Controls.Header.DropDownActionItem("kHydroCsharpDropdown", "DropDown");
            simpleDropDown.GroupCaption = groupCaption2;
            App.HeaderControl.Add(simpleDropDown);

            //Add a Ribbon Menu container item
            string menuKey = "kHydroCsharpMenuContainer";
            var simpleMenu = new MenuContainerItem(KHydroCSharp, menuKey, "Menu Container");
            simpleMenu.GroupCaption = groupCaption2;
            App.HeaderControl.Add(simpleMenu);
            
            var menuItem1 = new SimpleActionItem(KHydroCSharp, menuKey, "item 1", menuItem1_Click) { GroupCaption = groupCaption2, SmallImage = CreateSmallImage(Color.Green) };
            var menuItem2 = new SimpleActionItem(KHydroCSharp, menuKey, "item 2", menuItem2_Click) { GroupCaption = groupCaption2, SmallImage = CreateSmallImage(Color.Red) };

            //set the selected item
            menuItem1.Enabled = true;

            //TODO try implementing this via ribbon
            //RibbonItemGroup grp = new RibbonItemGroup();
            //rp2.Items.Add(grp);
            //grp.Items.Add(_rcb);

            ////Add a ribbon host
            //RibbonHost host = new RibbonHost();
            //host.HostedControl = new DateTimePicker();
            //rp2.Items.Add(host);

            ////Add a ribbon button list
            //RibbonPanel rp3 = new RibbonPanel();
            //_ribbonSampleTab.Panels.Add(rp3);

            //RibbonButtonList rblst = new RibbonButtonList();
            //rblst.ItemsWideInLargeMode = 100;
            //rblst.ControlButtonsWidth = 100;
            //rblst.DropDownItems.Add(new RibbonButton("vole"));
            //rblst.DropDownItems.Add(new RibbonButton("kravo"));
            //rp3.Items.Add(rblst);

            ////Add a ribbon button list
            //RibbonPanel rp4 = new RibbonPanel();
            //_ribbonSampleTab.Panels.Add(rp4);

            //RibbonHost host2 = new RibbonHost();
            //ListBox lstBx = new ListBox();
            //lstBx.Height = 50;
            //lstBx.Items.Add("vole");
            //lstBx.Items.Add("kravo");
            //lstBx.Items.Add("klokan");
            //lstBx.Items.Add("zajic");
            //host2.HostedControl = lstBx;
            //rp4.Items.Add(host2);

            //RibbonPanel rp5 = new RibbonPanel();
            //rp5.Text = "ribbon panel 5";
            //_ribbonSampleTab.Panels.Add(rp5);

            ////Add a Ribbon Text Box
            //_rtxt = new RibbonTextBox();
            ////_rtxt.MaxSizeMode = RibbonElementSizeMode.Compact;
            //_rtxt.TextAlignment = RibbonItem.RibbonItemTextAlignment.Left;

            //_rtxt.Text = "Enter Custom Setting:";
            //rp5.Items.Add(_rtxt);

            //assign the project saving and project loading Events
            App.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            App.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);

            // This line ensures that "Enabled" is set to true.
            base.Activate();
        }

        #endregion

        #region IPlugin Members

        private Image CreateCircleImage(Color imageColor)
        {
            Bitmap bmp = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(bmp);
            Brush brush = new SolidBrush(imageColor);
            g.FillEllipse(Brushes.Blue, new Rectangle(0, 0, 32, 32));
            g.Dispose();
            brush.Dispose();
            return bmp;
        }

        private Image CreateSmallImage(Color imageColor)
        {
            Bitmap bmp = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bmp);
            Brush brush = new SolidBrush(imageColor);
            g.FillEllipse(Brushes.Blue, new Rectangle(0, 0, 32, 32));
            g.Dispose();
            brush.Dispose();
            return bmp;
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            string customSettingValue = App.SerializationManager.GetCustomSetting<string>(_pluginName + "_Setting1","Enter Custom Setting:");
            //_rtxt.TextBoxText = customSettingValue;
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            //App.SerializationManager.SetCustomSetting(_pluginName + "_Setting1", _rtxt.TextBoxText);
        }

        #endregion

        #region Event Handlers

        //when user clicks the first ribbon button
        void rb_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Connection String Is: " + 
                HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, _pluginName);
        }

        void menuItem1_Click(object sender, EventArgs e)
        {

        }

        void menuItem2_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
