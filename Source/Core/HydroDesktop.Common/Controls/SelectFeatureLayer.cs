using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Symbology;

namespace HydroDesktop.Common.Controls
{
    /// <summary>
    /// Allow select feature layers
    /// </summary>
    public partial class SelectFeatureLayer : Form
    {
        /// <summary>
        /// Create new instance of SelectFeatureLayer
        /// </summary>
        /// <param name="layers">Layers to select</param>
        public SelectFeatureLayer(IEnumerable<IFeatureLayer> layers)
        {
            InitializeComponent();

            cmbLayers.ValueMember = "LegendText";
            cmbLayers.Items.Clear();
            foreach (var item in layers)
            {
                cmbLayers.Items.Add(item);
            }
            if (cmbLayers.Items.Count > 0)
            {
                cmbLayers.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Get selected layer
        /// </summary>
        public FeatureLayer SelectedLayer
        {
            get { return (FeatureLayer) cmbLayers.SelectedItem; }
        }
    }
}
