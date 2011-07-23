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
    public class Main : Extension, IMapPlugin
    {
        #region Variables
        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;
        //refers to the 'Table' root key
        private const string TableTabKey = "kTable";     
        #endregion

        #region IExtension Members

        protected override void OnDeactivate()
        {
            _mapArgs.AppManager.HeaderControl.RemoveItems();

            base.OnDeactivate();
        }

        #endregion

        #region IPlugin Members

        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;

            var btnWaterML = new SimpleActionItem("WaterML", menu_Click);
            btnWaterML.RootKey = TableTabKey;
            btnWaterML.LargeImage = Resources.waterml_import1;
            btnWaterML.GroupCaption = "Data Import";
            _mapArgs.AppManager.HeaderControl.Add(btnWaterML);
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
