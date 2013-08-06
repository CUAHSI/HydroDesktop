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
        /// The Series selector menu control in the Series View.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ISeriesSelector SeriesSelector { get; }
    }
}
