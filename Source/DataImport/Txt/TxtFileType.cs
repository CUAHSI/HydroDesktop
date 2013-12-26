using System.ComponentModel;

namespace DataImport.Txt
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