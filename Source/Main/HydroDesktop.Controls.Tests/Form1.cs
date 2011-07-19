using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;

namespace SeriesSelectorTest
{
    public partial class Form1 : Form
    {
        #region Constructor
        public Form1()
        {
            InitializeComponent();
            this.seriesSelector1.SeriesCheck += new ItemCheckEventHandler(seriesSelector1_SeriesCheck);
            this.seriesSelector1.CriterionChanged+=new EventHandler(seriesSelector1_CriterionChanged);


        }
        #endregion

        #region SeriesSelector Load
        private void seriesSelector1_Load(object sender, EventArgs e)
        {         
            //lblCheckState.Text = seriesSelector1.CheckedSeriesState.ToString();
        }
        #endregion
        private void seriesSelector1_SeriesCheck(object sender, ItemCheckEventArgs e)
        {
            //lblCheckState.Text = seriesSelector1.CheckedSeriesState.ToString();
        }
        private void seriesSelector1_CriterionChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("CriterionChanged");
        }
        private void seriesSelector1_ShowAllSeries(object sender, EventArgs e)
        {
            //MessageBox.Show("ShowAllSeries");
        }

        private void btnTestChange_Click(object sender, EventArgs e)
        {
            string deletedSeriesIDs = "";
            
            foreach (object seriesID in seriesSelector1.CheckedIDList)
            {
                //HydroDesktop.Database.Config.SQLRepositoryManager.DeleteSeries(Convert.ToInt32(seriesID));
                //deletedSeriesIDs += seriesID.ToString() + " ";
            }
            MessageBox.Show("The series " + deletedSeriesIDs + "are deleted. Press the Refresh button.");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //seriesSelector1.RefreshSelection();
        }

        private void bntSelect_Click(object sender, EventArgs e)
        {
            //seriesSelector1.SelectSeries(seriesSelector1._selectedSeriesId);
        }

        private void bntUnSelect_Click(object sender, EventArgs e)
        {
            //seriesSelector1.UnselectSeries(seriesSelector1.CheckedSeriesID);
        }

        private void bntCheckState_Click(object sender, EventArgs e)
        {
            //bntCheckState.Text = seriesSelector1.CheckedSeriesState.ToString();
        }


    }
}
