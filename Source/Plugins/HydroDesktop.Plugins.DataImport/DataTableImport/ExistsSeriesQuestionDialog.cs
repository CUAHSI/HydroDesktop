using System.Windows.Forms;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Plugins.DataImport.DataTableImport
{
    /// <summary>
    /// Question dialog in case when series exists
    /// </summary>
    public partial class ExistsSeriesQuestionDialog : Form
    {
        /// <summary>
        /// Create new instance of <see cref="ExistsSeriesQuestionDialog"/>
        /// </summary>
        /// <param name="siteName">Site name</param>
        /// <param name="varName">Variable name</param>
        public ExistsSeriesQuestionDialog(string siteName, string varName)
        {
            InitializeComponent();

            lblInfo.Text = string.Format("There is already a time series with site: [{0}] and variable: [{1}].",
                                         siteName, varName);
        }

        /// <summary>
        /// Current Overwrite option
        /// </summary>
        public OverwriteOptions CurrentOption
        {
            get
            {
                if (rbCreateNew.Checked)
                    return OverwriteOptions.Copy;
                if (rbOverwrite.Checked)
                    return OverwriteOptions.Overwrite;
                if (rbMerge.Checked)
                    return OverwriteOptions.Fill;
                return OverwriteOptions.Copy;
            }
        }
    }
}
