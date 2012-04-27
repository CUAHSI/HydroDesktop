using System;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;

namespace SeriesView
{
    public partial class SeriesProperties : Form
    {
        public SeriesProperties(Series series)
        {
            if (series == null) throw new ArgumentNullException("series");
            InitializeComponent();

            siteView1.ReadOnly = true;
            siteView1.Entity = series.Site;

            variableView1.ReadOnly = true;
            variableView1.Entity = series.Variable;

            methodView1.ReadOnly = true;
            methodView1.Entity = series.Method;

            sourceView1.ReadOnly = true;
            sourceView1.Entity = series.Source;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
           Close();
        }
    }
}
