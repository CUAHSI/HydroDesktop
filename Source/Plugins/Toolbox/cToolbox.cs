using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;

namespace Toolbox
{
    public partial class cToolbox : UserControl
    {
        public cToolbox()
        {
            InitializeComponent();
        }

        public ToolManager ToolManager
        {
            get { return toolManager1; }
        }

        private void toolManagerToolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
