using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;

namespace HydroDesktop.ArcGisOnline
{
    public partial class ArcGISOnlineForm : Form
    {
        public ArcGISOnlineForm()
        {
            InitializeComponent();
        }

        public AppManager App;

        private void btnOK_Click(object sender, EventArgs e)
        {
            //TODO: validate URL
            IFeatureSet fs = ServiceRequest.GetFeatures(this.textBox1.Text);

            fs.Projection = KnownCoordinateSystems.Geographic.World.WGS1984;

            App.Map.Layers.Add(fs);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }
    }
}
