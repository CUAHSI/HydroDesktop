using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
namespace RibbonSamplePlugin
{

    public class Main : Extension
    {
        #region Variables

        //reference to the main series view panel
        private ISeriesView _seriesView;

        //the name of the plugin displayed in the ribbon tab
        private const string _pluginName = "Ribbon Sample Plugin";

        //the ribbon tab added by the plugin
        private RibbonTab _ribbonSampleTab;

        //the ribbon button added by the plugin
        private RibbonButton _ribbonBtn;

        private RibbonComboBox _rcb;

        private RibbonTextBox _rtxt;

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            // Remove ribbon tab
            
            if (App.Ribbon.Tabs.Contains(_ribbonSampleTab))
            {
                App.Ribbon.Tabs.Remove(_ribbonSampleTab);
            }
            // Remove ribbon button in view panel
            App.Ribbon.Tabs[0].Panels[0].Items.Remove(_ribbonBtn);
            // Remove Work Page
            
            //remove the plugin panel           
            _seriesView.RemovePanel(_pluginName);          
            
            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        public override void Activate()
        {
            // Add "Ribbon" button to the "View" Panel in "Home" ribbon tab
            _ribbonBtn = new RibbonButton("Ribbon");
            App.Ribbon.Tabs[0].Panels[0].Items.Add(_ribbonBtn);
            Bitmap bmp = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(Brushes.Blue, new Rectangle(0, 0, 32, 32));
            _ribbonBtn.Image = bmp;
            g.Dispose();
            _ribbonBtn.Click += new EventHandler(ribbonButton_Click);

            // Add "Ribbon Sample Plugin" panel to the SeriesView
            IHydroAppManager manager = App as IHydroAppManager;
            if (manager != null)
            {
                _seriesView = manager.SeriesView;
                MyUserControl uc = new MyUserControl(_seriesView.SeriesSelector);
                _seriesView.AddPanel(_pluginName, uc);
            }

            // Add a new ribbon tab
            _ribbonSampleTab = new RibbonTab(App.Ribbon, _pluginName);

            // Create the Ribbon Button with a ribbon panel on the new ribbon tab
            RibbonPanel rp = new RibbonPanel();
            rp.Text = "ribbon panel";
            _ribbonSampleTab.Panels.Add(rp);
            RibbonButton rb = new RibbonButton("ribbon button");
            rp.Items.Add(rb);
            Bitmap bmpRb = new Bitmap(32, 32);
            Graphics gRb = Graphics.FromImage(bmpRb);
            gRb.FillEllipse(Brushes.Blue, new Rectangle(0, 0, 32, 32));
            rb.Image = bmpRb;
            gRb.Dispose();
            rb.Click += new EventHandler(rb_Click);

            //Add a "Ribbon Panel" 2
            RibbonPanel rp2 = new RibbonPanel();
            rp2.Text = "ribbon panel 2";
            //rp2.FlowsTo = RibbonPanelFlowDirection.Right;
            _ribbonSampleTab.Panels.Add(rp2);

            //Add a Ribbon Combo Box
            _rcb = new RibbonComboBox();
            _rcb.MaxSizeMode = RibbonElementSizeMode.Compact;

            _rcb.MouseDown += new MouseEventHandler(rcb_MouseDown);

            RibbonButton svcBtn = new RibbonButton();
            svcBtn.Text = "item 1";
            svcBtn.ToolTip = "item 1";
            //svcBtn.Style = RibbonButtonStyle.DropDownListItem;
            svcBtn.Style = RibbonButtonStyle.Normal;
            _rcb.DropDownItems.Add(svcBtn);

            RibbonButton svcBtn2 = new RibbonButton();
            svcBtn2.Text = "item 2";
            svcBtn2.ToolTip = "item 2";
            //svcBtn.Style = RibbonButtonStyle.DropDownListItem;
            svcBtn2.Style = RibbonButtonStyle.Normal;
            _rcb.DropDownItems.Add(svcBtn);

            RibbonButton svcBtn3 = new RibbonButton();
            svcBtn3.Text = "item 3";
            svcBtn3.ToolTip = "item 3";
            //svcBtn.Style = RibbonButtonStyle.DropDownListItem;
            svcBtn3.Style = RibbonButtonStyle.Normal;
            _rcb.DropDownItems.Add(svcBtn3);

            _rcb.SelectedItem = svcBtn3;

            RibbonItemGroup grp = new RibbonItemGroup();
            rp2.Items.Add(grp);
            grp.Items.Add(_rcb);

            //Add a ribbon host
            RibbonHost host = new RibbonHost();
            host.HostedControl = new DateTimePicker();
            rp2.Items.Add(host);

            //Add a ribbon button list
            RibbonPanel rp3 = new RibbonPanel();
            _ribbonSampleTab.Panels.Add(rp3);

            RibbonButtonList rblst = new RibbonButtonList();
            rblst.ItemsWideInLargeMode = 100;
            rblst.ControlButtonsWidth = 100;
            rblst.DropDownItems.Add(new RibbonButton("vole"));
            rblst.DropDownItems.Add(new RibbonButton("kravo"));
            rp3.Items.Add(rblst);

            //Add a ribbon button list
            RibbonPanel rp4 = new RibbonPanel();
            _ribbonSampleTab.Panels.Add(rp4);

            RibbonHost host2 = new RibbonHost();
            ListBox lstBx = new ListBox();
            lstBx.Height = 50;
            lstBx.Items.Add("vole");
            lstBx.Items.Add("kravo");
            lstBx.Items.Add("klokan");
            lstBx.Items.Add("zajic");
            host2.HostedControl = lstBx;
            rp4.Items.Add(host2);

            RibbonPanel rp5 = new RibbonPanel();
            rp5.Text = "ribbon panel 5";
            _ribbonSampleTab.Panels.Add(rp5);

            //Add a Ribbon Text Box
            _rtxt = new RibbonTextBox();
            //_rtxt.MaxSizeMode = RibbonElementSizeMode.Compact;
            _rtxt.TextAlignment = RibbonItem.RibbonItemTextAlignment.Left;

            _rtxt.Text = "Enter Custom Setting:";
            rp5.Items.Add(_rtxt);


            if (!App.Ribbon.Tabs.Contains(_ribbonSampleTab))
            {
                App.Ribbon.Tabs.Add(_ribbonSampleTab);
            }

            _ribbonSampleTab.ActiveChanged += new EventHandler(_ribbonSampleTab_ActiveChanged);

            //assign the project saving and project loading Events
            App.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            App.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);

            // This line ensures that "Enabled" is set to true.
            base.Activate();
        }

        #endregion

        #region IPlugin Members

        

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            string customSettingValue = App.SerializationManager.GetCustomSetting<string>(_pluginName + "_Setting1","Enter Custom Setting:");
            _rtxt.TextBoxText = customSettingValue;
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            App.SerializationManager.SetCustomSetting(_pluginName + "_Setting1", _rtxt.TextBoxText);
        }

        void rcb_MouseDown(object sender, MouseEventArgs e)
        {
            _rcb.ShowDropDown();
        }

        // When the ribbon tab is changed, the Series view panel is changed
        void _ribbonSampleTab_ActiveChanged(object sender, EventArgs e)
        {
            if (_ribbonSampleTab.Active)
            {
                if (App.TabManager != null)
                {
                    App.TabManager.SelectedTabName = "MapView";
                    if (_seriesView != null)
                    {
                        _seriesView.VisiblePanelName = "MapView";
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        //when user clicks the second ribbon button
        void ribbonButton_Click(object sender, EventArgs e)
        {
            if (App.TabManager.TabCount > 0)
            {
                MessageBox.Show("Connection String Is: " + 
                    HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, _pluginName);
            }
        }

        //when user clicks the first ribbon button
        void rb_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Connection String Is: " + 
                HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, _pluginName);
        }

        #endregion
    }
}
