using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;

namespace RibbonSamplePlugin
{
    public partial class MyUserControl : UserControl
    {
        //the series selection control
        private ISeriesSelector _seriesSelector;

        private ToolStripItem _contextMenuItem = null;
        
        public MyUserControl(ISeriesSelector seriesSelector)
        {
            InitializeComponent();

            //to access the map and database elements
            _seriesSelector = seriesSelector;

            //the SeriesChecked event
            _seriesSelector.SeriesCheck += new SeriesEventHandler(SeriesSelector_SeriesCheck);
            _seriesSelector.SeriesSelected += new EventHandler(SeriesSelector_SeriesSelected);

            //the checked IDs
            lstCheckedSeriesIDs.Items.Clear();
            foreach (int id in _seriesSelector.CheckedIDList)
            {
                lstCheckedSeriesIDs.Items.Add(id);
                lstCheckedSeriesIDs.SetSelected(lstCheckedSeriesIDs.Items.Count - 1, true);
            }

            //add the context menu item
            _contextMenuItem = _seriesSelector.ContextMenuStrip.Items.Add("Sample Context Menu Item");
            _contextMenuItem.Click += new EventHandler(contextMenuItem_Click);

            //the selected series ID
            lblSelectedID.Text = "SelectedSeriesID: " + _seriesSelector.SelectedSeriesID.ToString();
        }

        void SeriesSelector_SeriesSelected(object sender, EventArgs e)
        {
            lblSelectedID.Text = "SelectedSeriesID: " + _seriesSelector.SelectedSeriesID.ToString();

            listBoxEvents.Items.Add("SeriesSelected: " + _seriesSelector.SelectedSeriesID.ToString());
            listBoxEvents.SetSelected(listBoxEvents.Items.Count - 1, true);
        }

        //when the context menu item is clicked
        void contextMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You clicked the context menu item: ");
        }

        void SeriesSelector_SeriesCheck(object sender, SeriesEventArgs e)
        {
            listBoxEvents.Items.Add("SeriesCheck: " + e.SeriesID.ToString() + ", " + e.IsChecked.ToString());
            listBoxEvents.SetSelected(listBoxEvents.Items.Count - 1, true);

            lstCheckedSeriesIDs.Items.Clear();
            foreach (int id in _seriesSelector.CheckedIDList)
            {
                lstCheckedSeriesIDs.Items.Add(id);
                lstCheckedSeriesIDs.SetSelected(lstCheckedSeriesIDs.Items.Count - 1, true);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _seriesSelector.CheckBoxesVisible = checkBox1.Checked;
        }

        
    }
}
