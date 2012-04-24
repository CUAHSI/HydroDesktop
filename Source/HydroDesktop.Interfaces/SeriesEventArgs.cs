using System;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Event arguments for the SeriesChecked event
    /// </summary>
    public class SeriesEventArgs : EventArgs
    {
        private readonly int _seriesID;
        private readonly bool _isChecked;

        /// <summary>
        /// Creates a new instance of the series EventArgs
        /// </summary>
        /// <param name="seriesID">Series ID</param>
        /// <param name="isChecked">True if new staus is checked, 
        /// false if new status is unchecked</param>
        public SeriesEventArgs(int seriesID, bool isChecked)
        {
            _seriesID = seriesID;
            _isChecked = isChecked;
        }

        /// <summary>
        /// The checked or unchecked series ID
        /// </summary>
        public int SeriesID
        {
            get { return _seriesID;  }
        }

        /// <summary>
        /// The check state (checked or unchecked)
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
        }
    }

    #region Delegates
    /// <summary>
    /// Series checked event handler
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">information about checked or unchecked series</param>
    public delegate void SeriesEventHandler(object sender, SeriesEventArgs e);
    #endregion
        
}
