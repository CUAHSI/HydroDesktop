using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;

namespace HydroDesktop.MainApplication
{
    public partial class NavigationTools : Form
    {
        private Map map;
        public NavigationTools(Map map)
        {
            this.map = map;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            map.FunctionMode = FunctionMode.Pan;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            map.FunctionMode = FunctionMode.ZoomIn;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            map.FunctionMode = FunctionMode.ZoomOut;
        }
    }
}
