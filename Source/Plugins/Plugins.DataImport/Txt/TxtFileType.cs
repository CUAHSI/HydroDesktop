using System.ComponentModel;

namespace HydroDesktop.Plugins.DataImport.Txt
{
    /// <summary>
    /// Txt file type
    /// </summary>
    public enum TxtFileType
    {
        /// <summary>
        /// Fixed width
        /// </summary>
        [Description("Fixed width")]
        FixedWidth,

        /// <summary>
        /// Delimited
        /// </summary>
        [Description("Delimited")]
        Delimited,
    }
}