using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Topology;
using Search3;

namespace HydroDesktop.MainApplication
{
    public partial class NavigationControl : UserControl
    {
        private AppManager appManager;
        private Button button6;

        public NavigationControl(AppManager appManager)
        {
            this.appManager = appManager;
            button6 = new Button();
            button6.Size = Size.Empty;
            this.Controls.Add(button6);
            InitializeComponent();

            appManager.Map.FunctionModeChanged += Map_FunctionModeChanged;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Space:
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void Map_FunctionModeChanged(object sender, EventArgs e)
        {
            if (appManager.Map.FunctionMode == FunctionMode.Pan)
                button1.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.ZoomIn)
                button2.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.ZoomOut)
                button3.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.Select)
                button4.Focus();
            else
                button6.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            appManager.Map.FunctionMode = FunctionMode.Pan;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            appManager.Map.FunctionMode = FunctionMode.ZoomIn;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            appManager.Map.FunctionMode = FunctionMode.ZoomOut;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            appManager.Map.FunctionMode = FunctionMode.Select;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (appManager.Map.FunctionMode == FunctionMode.Pan)
                button1.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.ZoomIn)
                button2.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.ZoomOut)
                button3.Focus();
            else if (appManager.Map.FunctionMode == FunctionMode.Select)
                button4.Focus();
            else
                button6.Focus();

            IEnvelope env; 
            appManager.Map.MapFrame.ClearSelection(out env);
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button1, "Pan");
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.button2, "Zoom In");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.button3, "Zoom Out");
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.button4, "Select");
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.button5, "Deselect");
        }
    }
}
