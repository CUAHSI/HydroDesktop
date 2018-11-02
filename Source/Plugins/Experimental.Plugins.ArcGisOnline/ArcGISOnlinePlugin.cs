﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Symbology;



namespace HydroDesktop.ArcGisOnline
{
    public class ArcGisOnlinePlugin : Extension
    {
        #region Fields

        private const string RootKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;
        
        #endregion

        #region Properties
        #endregion

        #region Public methods

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            if (App == null) throw new ArgumentNullException("App");

            // Initialize menu
            var btnDownload = new SimpleActionItem("ArcGIS Online", DoRunArcGISOnline)
                                  {
                                      RootKey = HeaderControl.HomeRootItemKey,
                                      GroupCaption = "ArcGIS Online",
                                      LargeImage = Properties.Resources.arcgis_online_32,
                                      SmallImage = Properties.Resources.arcgis_online_16
                                  };
            App.HeaderControl.Add(btnDownload);

            base.Activate();
        }

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }

        #endregion

        #region Private methods

        private void DoRunArcGISOnline(Object sender, EventArgs e)
        {
            string path = App.SerializationManager.CurrentProjectDirectory;
            var fd = new OpenFileDialog();

            if (fd.ShowDialog() == DialogResult.OK)
            {
                string fn = fd.FileName;

                FeatureSet fs = new FeatureSet();
                fs.FilePath = fn;
                int nf = fs.Features.Count;
            }

            
            //ArcGISOnlineForm frm = new ArcGISOnlineForm();
            //frm.App = App;
            //frm.Show();
        }

        #endregion
    }
}

