using System;
using System.Windows.Forms;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Helper for <see cref="Control"/>
    /// </summary>
    public static class ControlHelper
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">Control</param>
        /// <param name="code">Action</param>
        public static void UIThread(this Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }
    }
}
