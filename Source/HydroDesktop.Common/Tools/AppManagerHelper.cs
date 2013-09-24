using System.Linq;
using DotSpatial.Controls;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Contains extension methods for <see cref="AppManager"/>
    /// </summary>
    public static class AppManagerHelper
    {
        /// <summary>
        /// Get active extension by it's type.
        /// </summary>
        /// <typeparam name="T">Extension type</typeparam>
        /// <param name="appManager">App manager</param>
        /// <returns>Instance of extension, or null (if extension not found).</returns>
        public static T GetExtension<T>(this AppManager appManager)
        {
            return appManager.Extensions.Where(ext => ext.IsActive).OfType<T>().FirstOrDefault();
        }
    }
}
