using System;
using DotSpatial.Symbology;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Contains extension methods for DotSpatial.Symbology namespace
    /// </summary>
    public static class SymbologyHelper
    {
        /// <summary>
        /// Add sub-menu item into parent menu item.
        /// </summary>
        /// <param name="parent">Parent menu item.</param>
        /// <param name="menuItemName">Menu item to add.</param>
        /// <param name="handler">Click event handler.</param>
        public static void AddMenuItem(this SymbologyMenuItem parent, string menuItemName, EventHandler handler)
        {
            if (parent.MenuItems.Exists(item => item.Name == menuItemName)) return;
            var menuItem = new SymbologyMenuItem(menuItemName, handler);
            parent.MenuItems.Add(menuItem);
        }
    }
}
