using System;
using System.ComponentModel;
using System.Linq.Expressions;
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


        /// <summary>
        /// Add binding to control
        /// </summary>
        /// <typeparam name="T">Type of control</typeparam>
        /// <typeparam name="TS">Type of object to bind</typeparam>
        /// <param name="control">Control</param>
        /// <param name="controlProperty">Expression that returns control property to bind</param>
        /// <param name="source">Bind source</param>
        /// <param name="sourceProperty">Expression that returns source property to bind</param>
        public static void AddBinding<T, TS>(this T control, Expression<Func<T, object>> controlProperty, object source, Expression<Func<TS, object>> sourceProperty)
            where T : Control
        {
            control.DataBindings.Add(new Binding(NameHelper<T>.Name(controlProperty),
                                                 source, NameHelper<TS>.Name(sourceProperty, true),
                                                 true, DataSourceUpdateMode.OnPropertyChanged));
        }

        /// <summary>
        /// Detecting Design Mode In Visual Studio
        /// </summary>
        /// <param name="control">Control</param>
        /// <returns>True - Design Mode, otherwise - False.</returns>
        public static bool IsDesignMode(this Control control)
        {
            var isDesignMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (isDesignMode) return true;

            var site = control.Site;
            if (site != null)
                isDesignMode = site.DesignMode;

            return isDesignMode;
        }
    }
}
