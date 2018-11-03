using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces;
using System.ComponentModel.Composition;

namespace SeriesView
{
    public partial class SeriesViewControl : UserControl, ISeriesView
    {
        public SeriesViewControl()
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

        }

        private void SetPanelVisible(string panelName)
        {

        }

        public void RemovePanel(string panelName)
        {

        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VisiblePanelName
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] PanelNames
        {
            get { return new string[] { }; }
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
