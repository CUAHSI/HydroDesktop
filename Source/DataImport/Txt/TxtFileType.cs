using System.ComponentModel;

namespace DataImport.Txt
{
    public enum TxtFileType
    {
        [Description("Fixed width")]
        FixedWidth,

        [Description("Delimited")]
        Delimited,
    }
}