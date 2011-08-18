using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace trmm
{
    public partial class frmResult2 : Form
    {
        private AppManager _app;

        public frmResult2()
        {
            InitializeComponent();
        }

        //get, or set the table
        public DataTable Table
        {
            get { return dataGridView1.DataSource as DataTable; }

            set { dataGridView1.DataSource = value; }
        }

        public AppManager App
        {
            get { return _app; }
            set { _app = value; }
        }

        public double ClickedLat { get; set; }

        public double ClickedLon { get; set; }
        

        private void button1_Click(object sender, EventArgs e)
        {
            
            Site site = new Site();
            site.Latitude = ClickedLat;
            site.Longitude = ClickedLon;
            site.Name = textBox1.Text;
            site.Code = textBox1.Text;
            site.SpatialReference = new SpatialReference("EPSG");
            site.NetworkPrefix = "trmm";

            Variable variable = new Variable();
            variable.TimeUnit = new Unit("day","time","d");
            variable.VariableUnit = new Unit("milimeter","length","mm");
            variable.Code = "pcp_mm";
            variable.DataType = "sum";
            variable.TimeSupport = 1;
            variable.ValueType = "sum";
            variable.SampleMedium = "air";
            variable.GeneralCategory = "meteorology";
            variable.Speciation = "Not Applicable";
            variable.VocabularyPrefix = "trmm";
            variable.Name = "precipitation";

            Source src = new Source();
            src.Organization = "NASA";
            src.Link = @"http://disc2.nascom.nasa.gov/Giovanni/tovas/";
            src.ISOMetadata = ISOMetadata.Unknown;
            
            Series s = new Series(site, variable, Method.Unknown, QualityControlLevel.Unknown, src);

            foreach(DataRow r in Table.Rows)
            {
                s.AddDataValue(Convert.ToDateTime(r[0]), Convert.ToDouble(r[1]));
            }
            s.ValueCount = s.DataValueList.Count;
            s.BeginDateTime = s.DataValueList[0].LocalDateTime;
            s.BeginDateTimeUTC = s.DataValueList[0].DateTimeUTC;
            s.EndDateTime = s.DataValueList[s.ValueCount - 1].LocalDateTime;
            s.EndDateTimeUTC = s.DataValueList[s.ValueCount - 1].DateTimeUTC;

            var manager = new RepositoryManagerSQL(DatabaseTypes.SQLite,
                HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString);
            manager.SaveSeries(s,new Theme("satellite precipitation"),OverwriteOptions.Append);

            var hydroManager = _app as IHydroAppManager;
            if (hydroManager != null)
            {
                hydroManager.SeriesView.SeriesSelector.RefreshSelection();
            }
        }
    }
}
