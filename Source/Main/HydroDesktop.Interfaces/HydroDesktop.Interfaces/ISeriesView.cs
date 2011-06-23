using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace HydroDesktop.Interfaces
{
    public interface ISeriesView
    {
        /// <summary>
        /// Creates a new panel and adds it as the top-most panel in the SeriesView
        /// </summary>
        /// <param name="panelName">The name of the panel</param>
        /// <param name="control">The user control displayed inside the panel</param>
        void AddPanel(string panelName, UserControl control);
        
        /// <summary>
        /// Removes an existing panel from the SeriesView. If the panel with the
        /// specified name does not exist, do not remove it
        /// </summary>
        /// <param name="panelName"></param>
        void RemovePanel(string panelName);
        
        /// <summary>
        /// Gets the name of the currently visible panel in the series view
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        string VisiblePanelName { get; set; }

        /// <summary>
        /// Gets the list of the names of all panels in the SeriesView
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        string[] PanelNames { get; }

        /// <summary>
        /// Event occurs when the visiblility of a panel in the series view is changed
        /// </summary>
        event EventHandler VisiblePanelChanged;
        
        /// <summary>
        /// The Series selector menu control in the Series View.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ISeriesSelector SeriesSelector { get; }
    }
}
