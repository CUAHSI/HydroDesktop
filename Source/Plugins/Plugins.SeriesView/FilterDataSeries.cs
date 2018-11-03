﻿using System;
using System.Collections;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.SeriesView
{
    public partial class FilterDataSeries : Form
    {
        #region Variables
        //Contains selected rule
        private readonly ArrayList filterOption = new ArrayList();
        //All the selected serieslist
        private readonly ArrayList seriesList = new ArrayList();
        #endregion

        #region Constructor
        public FilterDataSeries(ArrayList filterOption, ArrayList seriesList)
        {
            InitializeComponent();
            this.filterOption = filterOption;
            this.seriesList = seriesList;
        }
        #endregion

        #region Filter SeriesList
        /// <summary>
        /// Fill out the checkbox and combox when loading form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterDataSeries_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            string optionType = this.filterOption[0].ToString();

            switch (optionType)
            {
                case "Themes":
                    for (int i = 1; i < this.filterOption.Count; i++)
                    {
                        comboBox1.Items.Add(this.filterOption[i].ToString());
                    }
                    checkBox1.Checked = true;
                    break;
                case "Site":
                    for (int i = 1; i < this.filterOption.Count; i++)
                    {
                        comboBox2.Items.Add(this.filterOption[i].ToString());
                    }
                    checkBox2.Checked = true;
                    break;
                case "Variable":
                    for (int i = 1; i < this.filterOption.Count; i++)
                    {
                        comboBox3.Items.Add(this.filterOption[i].ToString());
                    }
                    checkBox3.Checked = true;
                    break;
                case "Method":
                    for (int i = 1; i < filterOption.Count; i++)
                    {
                        comboBox4.Items.Add(filterOption[i].ToString());
                    }
                    checkBox4.Checked = true;
                    break;
                case "Source":
                    for (int i = 1; i < filterOption.Count; i++)
                    {
                        comboBox5.Items.Add(filterOption[i].ToString());
                    }
                    checkBox5.Checked = true;
                    break;
                case "QCLevel":
                    for (int i = 1; i < this.filterOption.Count; i++)
                    {
                        comboBox6.Items.Add(this.filterOption[i].ToString());
                    }
                    checkBox6.Checked = true;
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        #endregion

        #region Filter Options
        /// <summary>
        /// Create a new selected set
        /// clears the selection and applies current filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntNew_Click(object sender, EventArgs e)
        {
            seriesList.Clear();
            filterOption.Clear();
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Select from set
        /// Applies the current filter to what is already in the selected set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntFromSet_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        
        /// <summary>
        ///  Add to set
        ///  Maintains the current selected set, filters the full list 
        ///  and adds matching series to the existing set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd2Set_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
