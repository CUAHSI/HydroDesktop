using System;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Topology;

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
            Controls.Add(button6);
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
            var ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(button1, "Pan");
            Cursor = Cursors.Arrow;
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            var ToolTip2 = new ToolTip();
            ToolTip2.SetToolTip(button2, "Zoom In");
            Cursor = Cursors.Arrow;
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            var ToolTip3 = new ToolTip();
            ToolTip3.SetToolTip(button3, "Zoom Out");
            Cursor = Cursors.Arrow;
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            var ToolTip4 = new ToolTip();
            ToolTip4.SetToolTip(button4, "Select");
            Cursor = Cursors.Arrow;
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            var ToolTip5 = new ToolTip();
            ToolTip5.SetToolTip(button5, "Deselect");
            Cursor = Cursors.Arrow;
        }

        private void NavigationControl_MouseHover(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}
