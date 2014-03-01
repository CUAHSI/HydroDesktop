using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Provides methods for selecting and unselecting data series in the series selector control
    /// and interacting with the HydroDesktop database
    /// </summary>
    public interface ISeriesSelector
    {
        /// <summary>
        /// Get the array of all checked series IDs
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int[] CheckedIDList { get; }

        /// <summary>
        /// Get the array of all visible series IDs as defined by the current filter
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int[] VisibleIDList { get; }

        /// <summary>
        /// Get the currently selected (highlighted) series ID. If no series is selected, 
        /// 0 is returned.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int SelectedSeriesID { get; set; }

        /// <summary>
        /// Get or set the currently used filter expression to limit the number of displayed
        /// series. If there is no filter expression used, returns a empty string
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        string FilterExpression { get; set; }

        /// <summary>
        /// Get the currently used type of filter (All, Simple, Complex)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        FilterTypes FilterType { get; }

        /// <summary>
        /// Get the context menu that appears on right-click of a series
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ContextMenuStrip ContextMenuStrip { get; }

        /// <summary>
        /// When set to true, the series check boxes are visible. When set to false, the series check
        /// boxes are not visible.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool CheckBoxesVisible { get; set; }

        /// <summary>
        /// Refresh all check boxes according to the status of the database. This method should be
        /// called when the data repository database is changed by a different application
        /// </summary>
        void RefreshSelection();
        /// <summary>
        /// Sets up the database (populates the series selector control by data series from the current
        /// database)
        /// </summary>
        void SetupDatabase();

        /// <summary>
        /// When a series is checked or unchecked
        /// </summary>
        event SeriesEventHandler SeriesCheck;

        /// <summary>
        /// When the refresh method is called or the 'Refresh' button is pressed
        /// </summary>
        event EventHandler Refreshed;

        /// <summary>
        /// Site column name.
        /// </summary>
        string SiteDisplayColumn { get; }
    }
}
