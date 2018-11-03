using DevExpress.XtraEditors;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class AttributeDataExplorerPlugin : Extension
	{
		private const string STR_KDataExplorer = "kDataExplorer";

		private bool isTerminating;

		private MainForm _MainForm;

		private DockablePanel _Panel;

		public AttributeDataExplorerPlugin()
		{
		}

        public override void Activate()
        {
            this._MainForm = new MainForm(this.App);
            this._Panel = new DockablePanel(STR_KDataExplorer, _MainForm.Text, _MainForm.hostpanel, DockStyle.Right);
            App.DockManager.Add(_Panel);

            // capture the event when the user closes the pane and unload the plugin.
            App.DockManager.PanelClosed += (sender, e) =>
            {
                if (e.ActivePanelKey == STR_KDataExplorer && !isTerminating)
                {
                    Deactivate();
                }
            };
            _MainForm.UILoaded();
            _MainForm.TextChanged += delegate(object sender, EventArgs e)
            {
                _Panel.Caption = _MainForm.Text;
            };
            base.Activate();
        }

		public override void Deactivate()
        {
            if (this._MainForm != null && !this._MainForm.IsDisposed)
            {
                isTerminating = true;

                this.App.DockManager.Remove(STR_KDataExplorer);
                this._MainForm.Dispose();
               
                isTerminating = false;
            }
            base.Deactivate();
        }
	}
}