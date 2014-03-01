using System.Drawing;
using System.Windows.Forms;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Contains misc helper methods for System.Drawing namespace
    /// </summary>
    public static class DrawingHelper
    {
        /// <summary>
        /// Show dialog to select color.
        /// </summary>
        /// <param name="defaultColor">Default color for dialog.</param>
        /// <returns>Selected color, or null, if dialog was cancelled.</returns>
        public static Color? PromptForColor(Color? defaultColor = null)
        {
            using(var dlg = new ColorDialog())
            {
                if (defaultColor != null)
                {
                    dlg.Color = defaultColor.Value;
                }

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.Color;
                }
                return null;
            }
        }
    }
}
