using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;

using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Controls;

namespace HydroDesktop.Main
{
    public partial class EditPluginsForm : Form
    {
        #region Variable
        private List<PluginToken> pluginTokens;
        private int formLeft = 0;
        private int formTop = 0;
        private AppManager _manager;

        private List<PluginToken> _selectPluginTokens = new List<PluginToken>();
        private List<PluginToken> _unSelectPluginTokens = new List<PluginToken>();
        #endregion

        #region properties
        public List<PluginToken> SelectPluginTokens
        {
            get { return _selectPluginTokens; }
        }

        public List<PluginToken> UnSelectPluginTokens
        {
            get { return _unSelectPluginTokens; }
        }
        #endregion

        public EditPluginsForm()
        {
            InitializeComponent();
        }

        public EditPluginsForm(DotSpatial.Controls.AppManager manager,int x, int y)
        {
            InitializeComponent();
            pluginTokens = manager.PluginTokens;
            _manager = manager;
            formLeft = x;
            formTop = y;
        }

        private void EditPluginsForm_Load(object sender, EventArgs e)
        {
            this.Left = formLeft - this.Width / 2;
            this.Top = formTop - this.Height / 2;
            foreach (PluginToken token in pluginTokens)
            {
                int index = listPlugins.Items.Add(token.Name);
                if (_manager.IsActive(token))
                {
                    listPlugins.SetItemChecked(index, true);
                    //record all the active plug-ins when the EditForm loads
                    _unSelectPluginTokens.Add(token);
                }
            }
        }

        private void listPlugins_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (listPlugins.SelectedItem != null)
            {
                foreach (PluginToken token in pluginTokens)
                {
                    if (token.Name == listPlugins.SelectedItem.ToString())
                    {
                       
                        this.rtbPluginInfo.Clear();
                        this.rtbPluginInfo.Font = new System.Drawing.Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
                        this.rtbPluginInfo.ForeColor = Color.Black;

                        this.rtbPluginInfo.AppendText("Name:    " + token.Name + "\n");
                        this.rtbPluginInfo.AppendText("Version: " + token.Version + "\n");
                        this.rtbPluginInfo.AppendText("Author:  " + token.Author + "\n");
                        this.rtbPluginInfo.AppendText("PluginType:  " + token.PluginType + "\n");
                        //this.rtbPluginInfo.AppendText("UniqueName:  " + token.UniqueName + "\n");
                    }
                }
            }
        }

        private void btnTurnAllOn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listPlugins.Items.Count;i++ )
            {
                listPlugins.SetItemChecked(i, true);
            }
        }

        private void btnTurnAllOff_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listPlugins.Items.Count; i++)
            {
                listPlugins.SetItemChecked(i, false);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            //add all selected plug-ins
            for (int i = 0; i < listPlugins.CheckedItems.Count; i++)
            {
                foreach (PluginToken token in pluginTokens)
                {
                    if (token.Name == listPlugins.CheckedItems[i].ToString())
                    {
                        _selectPluginTokens.Add(token);
                    }
                }
            }
            //add all unchecked plug-ins which are already active
            for (int i = 0; i < _unSelectPluginTokens.Count; i++)
            {
                if (_selectPluginTokens.Contains(_unSelectPluginTokens[i]))
                {
                    _unSelectPluginTokens.RemoveAt(i);
                    i--;
                }

            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
