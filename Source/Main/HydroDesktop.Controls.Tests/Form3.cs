using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SeriesSelectorTest
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            seriesSelector41.SeriesChecked += new HydroDesktop.ObjectModel.SeriesEventHandler(seriesSelector41_SeriesChecked);
            seriesSelector41.SeriesUnchecked += new HydroDesktop.ObjectModel.SeriesEventHandler(seriesSelector41_SeriesUnchecked);
        }

        void seriesSelector41_SeriesUnchecked(object sender, HydroDesktop.ObjectModel.SeriesEventArgs e)
        {
            listBox1.Items.Clear();
            foreach (int id in seriesSelector41.GetCheckedIDList())
            {
                listBox1.Items.Add(id);
            }
        }

        void seriesSelector41_SeriesChecked(object sender, HydroDesktop.ObjectModel.SeriesEventArgs e)
        {
            listBox1.Items.Clear();
            foreach (int id in seriesSelector41.GetCheckedIDList())
            {
                listBox1.Items.Add(id);
            }
        }
    }
}
