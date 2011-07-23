using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Event arguments for the SeriesChecked event
    /// </summary>
    public class SeriesEventArgs : EventArgs
    {
        private int _seriesID;
        private bool _isChecked;

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
    public delegate void SeriesEventHandler(object sender, SeriesEventArgs e);
    #endregion
        
}
