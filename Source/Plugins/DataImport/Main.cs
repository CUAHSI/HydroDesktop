using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using HydroDesktop.Database;
using DataImport;
using DotSpatial.Controls.Header;

//using HydroDesktop.Database.Model;
namespace ImportFromWaterML
{
    public class Main : Extension
    {
        #region Variables
        //refers to the 'Table' root key
        private const string TableTabKey = "kHydroTable";     
        #endregion

        #region IExtension Members

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }

        #endregion

        #region IPlugin Members

        public override void Activate()
        {
            var btnWaterML = new SimpleActionItem("WaterML", menu_Click);
            btnWaterML.RootKey = TableTabKey;
            btnWaterML.LargeImage = Resources.waterml_import1;
            btnWaterML.GroupCaption = "Data Import";
            App.HeaderControl.Add(btnWaterML);

            base.Activate();
        }

        #endregion

        #region Event Handlers

        void menu_Click(object sender, EventArgs e)
        {
            ImportDialog dlg = new ImportDialog();
            dlg.ShowDialog();
        }

        #endregion
    }
}
