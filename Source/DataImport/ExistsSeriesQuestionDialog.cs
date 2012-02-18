using System.Windows.Forms;
using HydroDesktop.Interfaces;

namespace DataImport
{
    public partial class ExistsSeriesQuestionDialog : Form
    {
        public ExistsSeriesQuestionDialog(string siteName, string varName)
        {
            InitializeComponent();

            lblInfo.Text = string.Format("There is already a time series with site: [{0}] and variable: [{1}].",
                                         siteName, varName);
        }

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
