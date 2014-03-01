using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace HydroDesktop.ZedGraphEx
{
    /// <summary>
    /// Improved ZedGraphControl.
    /// </summary>
    public class ZedGraphControlEx : ZedGraphControl
    {
        /// <summary>
        /// Create new instance of <see cref="ZedGraphControlEx"/>
        /// </summary>
        public ZedGraphControlEx()
        {
            ContextMenuBuilder += OnContextMenuBuilder;
        }

        private static void OnContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menustrip, Point mousept, ContextMenuObjectState objstate)
        {
            foreach (ToolStripItem item in menustrip.Items)
            {
                 if ("show_val".Equals(item.Tag) && String.Equals(item.Text, "Show Point Values", StringComparison.OrdinalIgnoreCase))
                 {
                     item.Text = "Show Tooltips on Hover";
                     break;
                 }
            }
        }

        #region Public methods

        /// <summary>
        /// Zoom In.
        /// </summary>
        public void ZoomIn()
        {
            ZedGraphControl_MouseWheel(this, new MouseEventArgs(MouseButtons.None,
                                                                1, Width/2, Height/2, 1));
        }

        #endregion
    }
}
