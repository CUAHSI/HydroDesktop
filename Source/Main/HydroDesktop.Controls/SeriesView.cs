using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Controls
{
    public partial class SeriesView : UserControl, ISeriesView
    {
        public SeriesView()
        {
            InitializeComponent();
        }

        private Dictionary<string, Panel> _panels = new Dictionary<string, Panel>();

        #region ISeriesView Members

        private void OnVisiblePanelChanged()
        {
            if (VisiblePanelChanged != null)
            {
                VisiblePanelChanged(this, null);
            }
        }

        public void AddPanel(string panelName, UserControl control)
        {
            string originalVisiblePanelName = VisiblePanelName;
            
            Panel newPanel = new Panel();
            newPanel.Name = panelName;
            newPanel.Dock = DockStyle.Fill;
            newPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            spcMain.Panel2.Controls.Add(newPanel);
            
            _panels.Add(panelName, newPanel);
            SetPanelVisible(panelName);

            if (VisiblePanelName != originalVisiblePanelName)
            {
                OnVisiblePanelChanged();
            }
        }

        private void SetPanelVisible(string panelName)
        {
            if (!_panels.ContainsKey(panelName)) return;

            string originalVisiblePanelName = VisiblePanelName;
            
            foreach (string name in _panels.Keys)
            {
                if (panelName == name)
                {
                    _panels[name].Visible = true;
                }
                else
                {
                    _panels[name].Visible = false;
                }
            }

            if (VisiblePanelName != originalVisiblePanelName)
            {
                OnVisiblePanelChanged();
            }
        }

        public void RemovePanel(string panelName)
        {
            string originalVisiblePanelName = VisiblePanelName;
            
            if (_panels.ContainsKey(panelName))
            {
                spcMain.Panel2.Controls.Remove(_panels[panelName]);
                _panels.Remove(panelName);

                int k = 0;
                foreach (Panel pnl in _panels.Values)
                {
                    if (k == 0)
                    {
                        pnl.Visible = true;
                    }
                    else
                    {
                        pnl.Visible = false;
                    }
                    k++;
                }
            }

            if (VisiblePanelName != originalVisiblePanelName)
            {
                OnVisiblePanelChanged();
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VisiblePanelName
        {
            get
            {
                foreach (string panelName in _panels.Keys)
                {
                    if (_panels[panelName].Visible) return panelName;
                }
                return string.Empty;
            }
            set
            {
                if (_panels.ContainsKey(value))
                {
                    SetPanelVisible(value);
                }
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] PanelNames
        {
            get { return _panels.Keys.ToArray<string>(); }
        }

        public event EventHandler VisiblePanelChanged;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISeriesSelector SeriesSelector
        {
            get { return seriesSelector1; }
        }

        #endregion
    }
}
